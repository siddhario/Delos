import { Component, Inject, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Artikal } from '../model/artikal';
import { Korisnik } from '../model/korisnik';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { fromEvent, Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError, map, filter } from 'rxjs/operators';
import { DomSanitizer } from '@angular/platform-browser';
import { ExcelService } from '../../services/export-excel-service';
import { Kategorija } from '../model/kategorija';

@Component({
  selector: 'app-korisnik',
  templateUrl: './artikal.component.html'
})
export class ArtikalComponent implements AfterViewInit {
  public artikli: Artikal[];
  sortOrder: boolean;
  sortColumn: string;
  searchText: string;
  loadAll: string = "0";
  currentUser: Korisnik;
  kategorije: Array<string> = new Array<string>();
  selectedKategorija: string;
  distinctKategorije: Set<string>;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef<any>;

  dobavljaci = ["--Svi--", "ASBIS", "AVTERA", "COMTRADE", "KIMTEC", "MINT", "UNIEXPERT"];
  kategorijeWebShop = [];
  artikliOriginal: Array<Artikal> = [];
  isSearching: boolean;
  selectedDostupnost: string = "1";
  http: HttpClient;
  baseUrl: string;
  selectedKategorijaWebShop: string;

  startSearch(naziv: string, selectedKategorijaWebShop: string, dostupnost: string, dobavljac: string, loadAll: string) {
    this.selectedKategorijaWebShop = selectedKategorijaWebShop;
    this.loadAll = loadAll;
    this.selectedDostupnost = dostupnost;
    //this.selectedKategorija = kategorija;
    this.selectedDobavljac = dobavljac;
    this.isSearching = true;
    if (selectedKategorijaWebShop == "--Sve--")
      selectedKategorijaWebShop = null;
    if (selectedKategorijaWebShop == "--Nekategorisano--")
      selectedKategorijaWebShop = "NULL";
    if (dobavljac == "--Svi--")
      dobavljac = null;

    this.http.get<Artikal[]>(this.baseUrl + 'webShopSync/artikliSearch?'
      + (naziv ? 'naziv=' + naziv : '')
      + (selectedKategorijaWebShop ? '&kategorija=' + selectedKategorijaWebShop : '')
      + (dostupnost ? '&dostupnost=' + dostupnost : '')
      + (dobavljac ? '&dobavljac=' + dobavljac : '')
      + (loadAll ? '&loadAll=' + loadAll : '')
    ).subscribe(result => {
      this.artikli = result;

      this.isSearching = false;
      //if (selectedKategorijaWebShop == null) {
      //  this.kategorije = new Array<string>();
      //  this.kategorije.push("--Sve--");

      //  this.artikli.forEach(a => {
      //    if (a.vrste != null)
      //      a.vrste.forEach(v => this.kategorije.push("[" + a.dobavljac + "] " + v));
      //  });
      //  let katgs = []
      //  katgs = this.kategorije.sort((a, b) => {
      //    if (a < b)
      //      return -1;
      //    else if (a > b)
      //      return 1;
      //    else
      //      return 0;
      //  })

      //  this.distinctKategorije = new Set(katgs);
      //}
    }, error => console.error(error));
  }
  selectedItem: Artikal;
  selectItem(artikal: Artikal) {
    if (this.selectedItem == artikal)
      this.selectedItem = null;
    else
      this.selectedItem = artikal;
  }
  sanitize(url: string) {
    return this.sanitizer.bypassSecurityTrustUrl(url);
  }
  ngAfterViewInit() {
    fromEvent(this.searchInput.nativeElement, 'keyup').pipe(
      // get value
      map((event: any) => {
        return event.target.value;
      })
      // if character length greater then 2
      , filter(res => res.length > 2 || res == "")
      // Time in milliseconds between key events
      , debounceTime(1000)
      // If previous query is diffent from current   
      , distinctUntilChanged()
      // subscription for response
    ).subscribe((text: string) => {
      this.startSearch(text, this.selectedKategorijaWebShop, this.selectedDostupnost, this.selectedDobavljac, this.loadAll);
    });

 
  }
  selectedDobavljac: string;


  sortProperty(property) {
    this.sortColumn = property;
    if (property == "sifra")
      this.sort(p => p.sifra, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "naziv")
      this.sort(p => p.naziv, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kategorija")
      this.sort(p => p.kategorija, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "dobavljac")
      this.sort(p => p.dobavljac, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "cijena_sa_rabatom")
      this.sort(p => p.cijena_sa_rabatom, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "cijena_prodajna")
      this.sort(p => p.cijena_prodajna, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "cijena_mp")
      this.sort(p => p.cijena_mp, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "dostupnost")
      this.sort(p => p.dostupnost, this.sortOrder == true ? "ASC" : "DESC");

  }
  sort<T>(prop: (c: Artikal) => T, order: "ASC" | "DESC"): void {
    this.artikli.sort((a, b) => {
      if (prop(a) < prop(b))
        return -1;
      if (prop(a) > prop(b))
        return 1;
      return 0;
    });

    if (order === "DESC") {
      this.artikli.reverse();
      this.sortOrder = true;
    } else {
      this.sortOrder = false;
    }
  }
  exportAsXLSX(): void {
    this.excelService.exportAsExcelFile(this.artikli, 'Artikli');
  }
  constructor(private excelService:ExcelService,private sanitizer: DomSanitizer, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private authenticationService: AuthenticationService, private router: Router) {
    this.baseUrl = baseUrl;
    this.http = http;
    this.sortColumn = 'naziv';
    this.sortOrder = true;
    this.selectedDobavljac = "--Svi--";
    this.selectedKategorijaWebShop = "--Sve--";
    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;
      if (this.currentUser == null)
        this.router.navigate(['/login']);
    });

    http.get<Kategorija[]>(baseUrl + 'webShopSync/kategorije').subscribe(result => {
      this.kategorijeWebShop = [{ naziv: "--Sve--" }, { naziv: "--Nekategorisano--" }];
      this.kategorijeWebShop = this.kategorijeWebShop.concat(result);
    }, error => console.error(error));

  }
}

