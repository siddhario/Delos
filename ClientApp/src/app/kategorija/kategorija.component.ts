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
  selector: 'app-kategorija',
  templateUrl: './kategorija.component.html'
})
export class KategorijaComponent {
  public kategorije: Kategorija[];
  sortOrder: boolean;
  sortColumn: string;
  searchText: string;
  loadAll: string = "0";
  currentUser: Korisnik;
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


  selectedItem: Kategorija;
  selectItem(kategorija: Kategorija) {
    if (this.selectedItem == kategorija)
      this.selectedItem = null;
    else
      this.selectedItem = kategorija;
  }
  sanitize(url: string) {
    return this.sanitizer.bypassSecurityTrustUrl(url);
  }

  selectedDobavljac: string;

  save(selectedItem: Kategorija) {
    selectedItem.marza= this.convertToNumber(selectedItem.marza);
    this.http.put<Kategorija>("webShopSync/updateKategorija", selectedItem).subscribe(result => {
      alert('OK');
    }, error => console.error(error));
  }


  sortProperty(property) {
    this.sortColumn = property;
    if (property == "sifra")
      this.sort(p => p.sifra, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "naziv")
      this.sort(p => p.naziv, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "marza")
      this.sort(p => p.marza, this.sortOrder == true ? "ASC" : "DESC");


  }

  convertToNumber(text: any) {
    if (text == undefined)
      return text;
    let n = +(text.toString().replace(",", "."));
    if (!Number.isNaN(n))
      return n;
    else
      return text;
  }
  sort<T>(prop: (c: Kategorija) => T, order: "ASC" | "DESC"): void {
    this.kategorije.sort((a, b) => {
      if (prop(a) < prop(b))
        return -1;
      if (prop(a) > prop(b))
        return 1;
      return 0;
    });

    if (order === "DESC") {
      this.kategorije.reverse();
      this.sortOrder = true;
    } else {
      this.sortOrder = false;
    }
  }

  azurirajKategorije() {
    this.http.get(this.baseUrl + 'webShopSync/updateKategorije').subscribe(result => {
      alert('OK');
    }, error => console.error(error));
  }
  exportAsXLSX(): void {
    this.excelService.exportAsExcelFile(this.kategorije, 'Kategorije');
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
      this.kategorije = result;
    }, error => console.error(error));

  }
}

