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
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgbdModalConfirm } from '../modal-focus/modal-focus.component';
import { ToastrService } from 'ngx-toastr';
import { istorija_cijena } from '../model/artikal - Copy';
import { QueryResult } from '../model/queryResult';

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
  selectedBrend: string;
  distinctBrendovi: Set<any>;
  brendovi: string[];
  selectedAktivan: string = "1";
  selectedKategorijeFilter = [];
  page: number = 1;
  pageCount: number;
  pages: Array<number>=[];
  getPageActiveClass(page) {
    if (page == this.page) {
      return "page-item active";
    }
    else
      return "page-item";
  }
  startSearch(naziv: string, selectedKategorijaWebShop: string, dostupnost: string, dobavljac: string, loadAll: string, brend: string, aktivan: string,page:number) {
    this.selectedKategorijaWebShop = selectedKategorijaWebShop;
    this.loadAll = loadAll;
    this.selectedDostupnost = dostupnost;
    //this.selectedKategorija = kategorija;
    this.selectedDobavljac = dobavljac;
    this.page = page;
    this.selectedBrend = brend;
    this.selectedAktivan = aktivan;

    this.isSearching = true;
    if (selectedKategorijaWebShop == "--Sve--")
      selectedKategorijaWebShop = null;
    if (selectedKategorijaWebShop == "--Nekategorisano--")
      selectedKategorijaWebShop = "NULL";
    if (dobavljac == "--Svi--")
      dobavljac = null;
    if (brend == "--Svi--")
      brend = null;
    if (aktivan == "--Svi--")
      aktivan = null;


    this.http.get<QueryResult>(this.baseUrl + 'webShopSync/artikliSearch?'
      + (naziv ? 'naziv=' + naziv : '')
      + (selectedKategorijaWebShop ? '&kategorija=' + selectedKategorijaWebShop : '')
      + (dostupnost ? '&dostupnost=' + dostupnost : '')
      + (dobavljac ? '&dobavljac=' + dobavljac : '')
      + (loadAll ? '&loadAll=' + loadAll : '')
      + (brend ? '&brend=' + brend : '')
      + (aktivan ? '&aktivan=' + aktivan : '')
      + ('&page=' + this.page)
      + ('&pageSize=50')
    ).subscribe(result => {

      this.pages = [];
    
      for (let i = 0; i < result.pageCount; i++) {
        this.pages.push(i+1);
      }
      this.artikli = result.data;
      this.pageCount = result.pageCount;

      this.isSearching = false;

      this.selectedKategorijeFilter = new Array<string>();
      this.selectedKategorijeFilter.push("--Svi--");

      this.artikli.forEach(a => {
        this.selectedKategorijeFilter.push(a.kategorija);
      });
      let katgs = []
      katgs = this.selectedKategorijeFilter.sort((a, b) => {
        if (a < b)
          return -1;
        else if (a > b)
          return 1;
        else
          return 0;
      })

      this.distinctKategorije = new Set(katgs);


      if (brend == null) {
        this.brendovi = new Array<string>();
        this.brendovi.push("--Svi--");

        this.artikli.forEach(a => {
          this.brendovi.push(a.brend);
        });
        let brnds = []
        brnds = this.brendovi.sort((a, b) => {
          if (a < b)
            return -1;
          else if (a > b)
            return 1;
          else
            return 0;
        })

        this.distinctBrendovi = new Set(brnds);
      }
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
      this.startSearch(text, this.selectedKategorijaWebShop, this.selectedDostupnost, this.selectedDobavljac, this.loadAll, this.selectedBrend, this.selectedAktivan, 1);
    });


  }
  selectedDobavljac: string;


  sortProperty(property) {
    this.sortColumn = property;
    if (property == "sifra")
      this.sort(p => p.sifra, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "barkod")
      this.sort(p => p.barkod, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "aktivan")
      this.sort(p => p.aktivan, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "naziv")
      this.sort(p => p.naziv, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kategorija")
      this.sort(p => p.kategorija, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "dobavljac")
      this.sort(p => p.dobavljac, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "brend")
      this.sort(p => p.brend, this.sortOrder == true ? "ASC" : "DESC");
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
      if (prop(a) < prop(b) || prop(a) == null)
        return -1;
      if (prop(a) > prop(b) || prop(b) == null)
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

  sortIstorija(istorija: Array<istorija_cijena>): Array<istorija_cijena> {
    istorija.sort((a, b) => {
      if (a.vrijeme < b.vrijeme)
        return 1;
      else
        return -1;
    });
    return istorija;
  }

  toggleAktivan(artikal: Artikal) {
    let modalRef = this.modalService.open(NgbdModalConfirm);
    modalRef.result.then((data) => {
      artikal.aktivan = !artikal.aktivan;
      this.http.put<Kategorija>("webShopSync/updateArtikal", artikal).subscribe(result => {
        this.toastr.success("Artikal je uspješno izmjenjen..");
      }, error => console.error(error));
    });

    modalRef.componentInstance.confirmText = "Da li ste sigurni da želite " + (artikal.aktivan == true ? "deaktivirati" : "aktivirati") + " artikal  " + artikal.naziv + " ? ";


  }
  exportAsXLSX(): void {
    this.excelService.exportAsExcelFile(this.artikli, 'Artikli');
  }

  hasRole(rola: string) {
    return this.currentUser.role.includes(rola);
  }

  constructor(private modalService: NgbModal, private toastr: ToastrService, private excelService: ExcelService, private sanitizer: DomSanitizer, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private authenticationService: AuthenticationService, private router: Router) {
    this.baseUrl = baseUrl;
    this.http = http;
    this.sortColumn = 'naziv';
    this.sortOrder = true;
    this.selectedDobavljac = "--Svi--";
    this.selectedKategorijaWebShop = "--Sve--";
    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;
      if (this.currentUser == null || (this.currentUser.role.includes('ADMIN')==false && this.currentUser.role.includes("WEBSHOP") == false && this.currentUser.role.includes("WEBSHOP_ADMIN") == false) )
        this.router.navigate(['/login']);
    });

    http.get<Kategorija[]>(baseUrl + 'webShopSync/kategorije').subscribe(result => {
      this.kategorijeWebShop = [{ naziv: "--Sve--" }, { naziv: "--Nekategorisano--" }];
      this.kategorijeWebShop = this.kategorijeWebShop.concat(result);
    }, error => console.error(error));

  }
}

