import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Korisnik } from '../model/korisnik';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { korisnikDetailsComponent } from '../korisnik-details/korisnik-details.component';

@Component({
  selector: 'app-korisnik',
  templateUrl: './korisnik.component.html'
})
export class KorisnikComponent {
  public korisnici: Korisnik[];
  baseUrl: string;
  http: HttpClient;
  currentUser: Korisnik;
  public selectedKorisnik: Korisnik;
  load() {
    this.http.get<Korisnik[]>(this.baseUrl + 'korisnik').subscribe(result => {
      this.korisnici = result;
    }, error => console.error(error));
  }

  add() {
    let modalRef = this.modalService.open(korisnikDetailsComponent
      , {
        size: 'lg',
        windowClass: 'modal-xl',
        backdrop: 'static'
      }
    );
    modalRef.componentInstance.itemAdd = true;
    modalRef.result.then((data) => {

      this.load();

    }, (reason) => {
      this.load();
    });
  }
  selectItem(korisnik: Korisnik) {

    //this.partneri.filter(dd => dd.sifra != partner.sifra).forEach((value) => { value.selected = false });
    korisnik.selected = !korisnik.selected;
    if (korisnik.selected == true)
      this.selectedKorisnik = korisnik;

    let modalRef = this.modalService.open(korisnikDetailsComponent
      , {
        size: "xl",
        windowClass: 'modal-xl'
      });
    modalRef.componentInstance.selectedKorisnik = korisnik;
    modalRef.result.then((data) => {

      this.load();

    }, (reason) => {
      this.load();
    });
  }
  constructor(private router: Router, httpClient: HttpClient, private authenticationService: AuthenticationService, @Inject('BASE_URL') baseUrl: string, private modalService: NgbModal ) {
    this.http = httpClient;
    this.baseUrl = baseUrl;
    this.load();

    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;
      if (this.currentUser == null)
        this.router.navigate(['/login']);
    });
  }
}

