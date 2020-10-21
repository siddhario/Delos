import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Korisnik } from '../model/korisnik';
import { NgbdModalConfirm } from '../modal-focus/modal-focus.component';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-korisnik-details',
  templateUrl: './korisnik-details.component.html'
})


export class korisnikDetailsComponent {

  public selectedKorisnik: Korisnik;
  itemEdit: boolean = false;
  public itemAdd: boolean = false;
  currentUser: Korisnik;
  izmjenaLozinke: boolean;
  izmijeniLozinku() {
    this.izmjenaLozinke = true;
  }
  saveKorisnik(korisnik: Korisnik) {

    if (korisnik.korisnicko_ime.length < 6) {
      this.toastr.error("Korisničko ime mora sadržavati minimum 6 karaktera!");
      return;
    }

    if (korisnik.ime.length < 1) {
      this.toastr.error("Ime korisnika je obavezno za unos!");
      return;
    }

    if (korisnik.prezime.length < 1) {
      this.toastr.error("Prezime korisnika je obavezno za unos!");
      return;
    }

    if (this.itemAdd == true || this.izmjenaLozinke==true) {
      if (korisnik.lozinka != korisnik.lozinkaPonovo) {
        this.toastr.error("Neodgovarajuća lozinka!");
        return;
      }
      if (korisnik.lozinka.length < 6) {
        this.toastr.error("Lozinka mora sadržavati minimum 6 karaktera!");
        return;
      }
      if (this.izmjenaLozinke == true) {
      
      
        this.http.post<Korisnik>(this.baseUrl + 'korisnik/izmjenaLozinke', korisnik).subscribe(result => {
          console.log("OK");
          this.toastr.success("Lozinka je uspješno promijenjena..");
          this.activeModal.close("OK");
        }, error => {
          this.toastr.error("Neodgovarajuća lozinka!");
          console.error(error)
        });
        return;
      }

      this.http.post<Korisnik>(this.baseUrl + 'korisnik', korisnik).subscribe(result => {
        console.log("OK");
        this.toastr.success("Korisnik je uspješno dodat..");
        this.activeModal.close("OK");
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    } else {
      //korisnik.datum = new Date(korisnik.datum.getFullYear(), korisnik.datum.getMonth(), korisnik.datum.getDate());
      this.http.put<Korisnik>(this.baseUrl + 'korisnik', korisnik).subscribe(result => {
        console.log("OK");
        this.toastr.success("Korisnik je uspješno izmijenjen..");
        this.activeModal.close("OK");
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
      //this.activeModal.close();
    }
  }

  delete(korisnicko_ime) {
    let modalRef = this.modalService.open(NgbdModalConfirm);
    modalRef.result.then((data) => {
    this.http.delete(this.baseUrl + 'korisnik?korisnickoIme=' + korisnicko_ime).subscribe(result => {
      console.log("OK");
      this.toastr.success("Korisnik je uspješno obrisan..");
      this.activeModal.close("DELETE");
    }, error => {
      this.toastr.error("Greška..");
      console.error(error)
    });
    }, (reason) => {
    });

    modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati korisnika \"" + korisnicko_ime + "\"?";
  }




  reloadItem(continueAdd: boolean) {
    this.http.get<Korisnik>(this.baseUrl + 'ponuda/getbybroj?broj=' + this.selectedKorisnik.korisnicko_ime).subscribe(result => {
      this.selectedKorisnik = result;

    }, error => {
      this.toastr.error("Greška..");
      console.error(error)
    });
  }
  cancel() {
    this.activeModal.close();
  }




  rowClass(ponuda: Korisnik) {
    if (ponuda.selected)
      return 'rowSelected';
    else
      return 'row';
  }

  constructor(private router: Router, private authenticationService: AuthenticationService,public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private modalService: NgbModal, public activeModal: NgbActiveModal, private toastr: ToastrService) {
    this.startAdd();
    this.izmjenaLozinke = false;
    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;
      if (this.currentUser == null)
        this.router.navigate(['/login']);
    });
  }

  startAdd() {
    this.selectedKorisnik = new Korisnik();
    //this.selectedPartner.status = "E";
    //this.selectedPartner.radnik = "dario";
    //this.selectedPartner.datum = (new Date());
  }


}

