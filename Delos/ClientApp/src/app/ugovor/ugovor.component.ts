import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Ugovor } from '../model/ugovor';
import { AuthenticationService } from '../auth/auth.service';
import { Korisnik } from '../model/korisnik';
import { Router } from '@angular/router';
import { fromEvent } from 'rxjs';
import { map, filter, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UgovorDetailsComponent } from '../ugovor-details/ugovor-details.component';
import { PregledUplataComponent } from '../pregledUplata/pregledUplata.component';
import { PregledDugovanjaComponent } from '../pregledDugovanja/pregledDugovanja.component';

@Component({
  selector: 'app-ugovor',
  templateUrl: './ugovor.component.html'
})
export class UgovorComponent {
  public ugovori: Ugovor[];

  searchText: string;
  currentUser: Korisnik;
  @ViewChild('searchInput', { static: false }) searchInput: ElementRef<any>;
  http: HttpClient;
  baseUrl: string;
  sortOrder: boolean;
  sortColumn: string;
  sortProperty(property) {
    this.sortColumn = property;
    if (property == "broj")
      this.sort(p => p.broj, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "datum")
      this.sort(p => p.datum, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kupac_naziv")
      this.sort(p => p.kupac_naziv, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kupac_adresa")
      this.sort(p => p.kupac_adresa, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kupac_telefon")
      this.sort(p => p.kupac_telefon, this.sortOrder == true ? "ASC" : "DESC");
  }
  sort<T>(prop: (c: Ugovor) => T, order: "ASC" | "DESC"): void {
    this.ugovori.sort((a, b) => {
      if (prop(a) < prop(b) || prop(a) == null)
        return -1;
      if (prop(a) > prop(b) || prop(b) == null)
        return 1;
      return 0;
    });

    if (order === "DESC") {
      this.ugovori.reverse();
      this.sortOrder = true;
    } else {
      this.sortOrder = false;
    }
  }



  startSearch(naziv: string) {
    this.http.get<Ugovor[]>(this.baseUrl + 'ugovor/search?'
      + (naziv ? 'naziv=' + naziv : '')
    ).subscribe(result => {
      this.ugovori = result;

    }, error => console.error(error));
  }
  selectedItem: Ugovor;
  selectItem(ugovor: Ugovor) {
    //this.prijave.filter(dd => dd.broj != prijava.broj).forEach((value) => { value.selected = false });
    if (this.selectedItem == ugovor)
      this.selectedItem = null;
    else
      this.selectedItem = ugovor;


    let modalRef = this.modalService.open(UgovorDetailsComponent
      , {
        size: "xl",
        windowClass: 'modal-xl'
      });
    modalRef.componentInstance.selectedUgovor = ugovor;
    modalRef.result.then((data) => {

      this.load();

    }, (reason) => {
      this.load();
    });
  }
  add() {
    let modalRef = this.modalService.open(UgovorDetailsComponent
      , {
        size: "xl",
        windowClass: 'modal-xl',
        backdrop: 'static'
      }
    );
    modalRef.componentInstance.startAdd();
    modalRef.result.then((data) => {
      this.load();
    }, (reason) => {
      this.load();
    });
  }
  load() {
    this.http.get<Ugovor[]>(this.baseUrl + 'ugovor').subscribe(result => {
      this.ugovori = result;
      let s;
    }, error => console.error(error));

  }
  pregledUplata() {
    let modalRef = this.modalService.open(PregledUplataComponent);
  }

  pregledDugovanja() {
    let modalRef = this.modalService.open(PregledDugovanjaComponent);
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
