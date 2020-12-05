import { Component, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Prijava } from '../model/prijava';
import { AuthenticationService } from '../auth/auth.service';
import { Korisnik } from '../model/korisnik';
import { Router } from '@angular/router';
import { fromEvent } from 'rxjs';
import { map, filter, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrijavaDetailsComponent } from '../prijava-details/prijava-details.component';

@Component({
  selector: 'app-prijava',
  templateUrl: './prijava.component.html'
})
export class PrijavaComponent {
  public prijave: Prijava[];

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
  //ngAfterViewInit() {
  //  fromEvent(this.searchInput.nativeElement, 'keyup').pipe(
  //    // get value
  //    map((event: any) => {
  //      return event.target.value;
  //    })
  //    // if character length greater then 2
  //    , filter(res => res.length > 2 || res == "")
  //    // Time in milliseconds between key events
  //    , debounceTime(1000)
  //    // If previous query is diffent from current   
  //    , distinctUntilChanged()
  //    // subscription for response
  //  ).subscribe((text: string) => {
  //    this.startSearch(text);
  //  });


  //}


  startSearch(naziv: string) {
    this.http.get<Prijava[]>(this.baseUrl + 'prijava/search?'
      + (naziv ? 'naziv=' + naziv : '')
    ).subscribe(result => {
      this.prijave = result;

    }, error => console.error(error));
  }
  selectedItem: Prijava;
  selectItem(prijava: Prijava) {
    //this.prijave.filter(dd => dd.broj != prijava.broj).forEach((value) => { value.selected = false });
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

      this.load();

    }, (reason) => {
      this.load();
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
      this.load();
    }, (reason) => {
      this.load();
    });
  }
  load() {
    this.http.get<Prijava[]>(this.baseUrl + 'prijava').subscribe(result => {
      this.prijave = result;
    }, error => console.error(error));

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
