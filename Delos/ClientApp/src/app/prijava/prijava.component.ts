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
      this.startSearch(text);
    });


  }


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
    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;
      if (this.currentUser == null)
        this.router.navigate(['/login']);
    });

    this.load();
  }
}
