import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Prijava } from '../model/prijava';
import { AuthenticationService } from '../auth/auth.service';
import { Korisnik } from '../model/korisnik';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrijavaDetailsComponent } from '../prijava-details/prijava-details.component';
import { QueryResult } from '../model/queryResult';

@Component({
  selector: 'app-prijava',
  templateUrl: './prijava.component.html'
})
export class PrijavaComponent {
  public prijave: Prijava[] = [];

  searchText: string;
  currentUser: Korisnik;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef<any>;
  http: HttpClient;
  baseUrl: string;
  sortOrder: boolean;
  sortColumn: string;
  page: number = 1;
  loading: boolean;
  pageCount: number;
  sortProperty(property) {
    this.sortColumn = property;
    if (property == "broj")
      this.sort(p => p.broj, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "datum")
      this.sort(p => p.datum, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kupac_ime")
      this.sort(p => p.kupac_ime, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kupac_adresa")
      this.sort(p => p.kupac_adresa, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kupac_telefon")
      this.sort(p => p.kupac_telefon, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "predmet")
      this.sort(p => p.predmet, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "model")
      this.sort(p => p.model, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "dobavljac_sifra")
      this.sort(p => p.dobavljac_sifra, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "poslat_mejl_dobavljacu")
      this.sort(p => p.poslat_mejl_dobavljacu, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "datum_vracanja")
      this.sort(p => p.datum_vracanja, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "zavrseno")
      this.sort(p => p.zavrseno, this.sortOrder == true ? "ASC" : "DESC");
  }
  sort<T>(prop: (c: Prijava) => T, order: "ASC" | "DESC"): void {
    this.prijave.sort((a, b) => {
      if (prop(a) < prop(b) || prop(a) == null)
        return -1;
      if (prop(a) > prop(b) || prop(b) == null)
        return 1;
      return 0;
    });

    if (order === "DESC") {
      this.prijave.reverse();
      this.sortOrder = true;
    } else {
      this.sortOrder = false;
    }
  }

  selectedItem: Prijava;
  selectItem(prijava: Prijava) {
    if (this.selectedItem == prijava)
      this.selectedItem = null;
    else
      this.selectedItem = prijava;


    let modalRef = this.modalService.open(PrijavaDetailsComponent
      , {
        size: "xl",
        windowClass: 'modal-xl'
      });
    modalRef.componentInstance.selectedPrijava = prijava;
    modalRef.result.then((data) => {

      this.search();

    }, (reason) => {
        this.search();
    });
  }
  add() {
    let modalRef = this.modalService.open(PrijavaDetailsComponent
      , {
        size: "xl",
        windowClass: 'modal-xl',
        backdrop: 'static'
      }
    );
    modalRef.componentInstance.startAdd();
    modalRef.result.then((data) => {
      this.search();
    }, (reason) => {
      this.search();
    });
  }
  load() {
    this.loading = true;
    this.http.get<QueryResult>(this.baseUrl + 'prijava?'
      + ('page=' + this.page)
      + ('&pageSize=50')
      + (this.searchText ? '&searchText=' + this.searchText : '')).subscribe(result => {
        this.loading = false;
        if (result.data != null)
          this.prijave = this.prijave.concat(result.data);
        if (this.page == 1)
          this.pageCount = result.pageCount;
      }, error => { console.error(error); this.loading = false; });

  }
  search() {
    this.page = 1;
    this.prijave = [];
    console.log(this.searchText);
    this.load();
  }
  onScroll() {
    if (this.page < this.pageCount) {
      this.page = this.page + 1;
      this.load();
    }
  }
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private authenticationService: AuthenticationService, private router: Router, private modalService: NgbModal) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.sortColumn = 'broj';
    this.sortOrder = true;
    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;
      if (this.currentUser == null)
        this.router.navigate(['/login']);
    });

    this.load();
  }
}
