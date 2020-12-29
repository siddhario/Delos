import { Component, Inject, Query } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { PonudaDetailsComponent } from '../ponuda-details/ponuda-details.component';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { AuthenticationService } from '../auth/auth.service';
import { Ponuda } from '../model/ponuda';
import { Korisnik } from '../model/korisnik';
import { Router } from '@angular/router';
import { QueryResult } from '../model/queryResult';
@Component({
  selector: 'app-ponuda',
  templateUrl: './ponuda.component.html'
})
export class PonudaComponent {
  public ponude: Ponuda[] = [];
  public selectedPonuda: Ponuda;
  itemAdd: boolean = false;
  sortOrder: boolean;
  sortColumn: string;
  searchText: string;
  currentUser: Korisnik;
  loading: boolean = false;
  constructor(private router: Router, private authenticationService: AuthenticationService, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private modalService: NgbModal) {
    this.page = 1;
    this.load();
    this.sortColumn = 'broj';
    this.sortOrder = true;

    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;

      if (this.currentUser == null || (this.currentUser.role.includes('ADMIN') == false && this.currentUser.role.includes("PONUDE") == false))
        this.router.navigate(['/login']);
    });
  }

  onScroll() {
    if (this.page < this.pageCount) {
      this.page = this.page + 1;
      this.load();
    }
  }

  sortProperty(property) {
    this.sortColumn = property;
    if (property == "broj")
      this.sort(p => p.broj, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "datum")
      this.sort(p => p.datum, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "partner_naziv")
      this.sort(p => p.partner_naziv, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "partner_adresa")
      this.sort(p => p.partner_adresa, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "partner_telefon")
      this.sort(p => p.partner_telefon, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "iznos_sa_rabatom")
      this.sort(p => p.iznos_sa_rabatom, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "pdv")
      this.sort(p => p.pdv, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "iznos_sa_pdv")
      this.sort(p => p.iznos_sa_pdv, this.sortOrder == true ? "ASC" : "DESC");
  }
  sort<T>(prop: (c: Ponuda) => T, order: "ASC" | "DESC"): void {
    this.ponude.sort((a, b) => {
      if (prop(a) < prop(b) || prop(a) == null)
        return -1;
      if (prop(a) > prop(b) || prop(b) == null)
        return 1;
      return 0;
    });

    if (order === "DESC") {
      this.ponude.reverse();
      this.sortOrder = true;
    } else {
      this.sortOrder = false;
    }
  }
  add() {
    let modalRef = this.modalService.open(PonudaDetailsComponent
      , {
        size: "xl",
        windowClass: 'modal-xl',
        backdrop: 'static'
      }
    );
    modalRef.componentInstance.startAdd();
    modalRef.result.then((data) => {
      if (data == 2)//reload items
        this.search();
      else if (data == 1) {//reload selected item
      }
    }, (reason) => {
      //this.ponude = [];
      //this.load();
    });
  }
  search() {
    this.page = 1;
    this.ponude = [];
    console.log(this.searchText);
    this.load();
  }
  modalRef: NgbModalRef;
  selectItem(ponuda: Ponuda) {

    this.ponude.filter(dd => dd.broj != ponuda.broj).forEach((value) => { value.selected = false });
    ponuda.selected = !ponuda.selected;
    if (ponuda.selected == true)
      this.selectedPonuda = ponuda;

    let modalRef = this.modalService.open(PonudaDetailsComponent
      , {
        size: "xl",
        windowClass: 'modal-xl',
        backdrop: 'static'
      });
    modalRef.componentInstance.selectedPonuda = ponuda;
    this.modalRef = modalRef;
    modalRef.result.then((data) => {
      //this.search();
      if (data == 2)//reload items
        this.search();
      else if (data == 1) {//reload selected item
        this.search();
      }
    }, (reason) => {

    });
  }
  page: number = 1;
  pageCount: number;
  load() {
    this.loading = true;
    this.http.get<QueryResult>(this.baseUrl + 'ponuda?'
      + ('page=' + this.page)
      + ('&pageSize=50')
      + (this.searchText ? '&searchText=' + this.searchText : '')).subscribe(result => {
        //this.http.get<Ponuda[]>(this.baseUrl + 'ponuda').subscribe(result => {
        //this.ponude = result;
        this.loading = false;
        if (result.data != null)
          this.ponude = this.ponude.concat(result.data);
        if (this.page == 1)
          this.pageCount = result.pageCount;
        //this.pages = [];
        //for (let i = 0; i < result.pageCount; i++) {
        //  this.pages.push(i + 1);
        //}
      }, error => { console.error(error); this.loading = false; });
  }
}


