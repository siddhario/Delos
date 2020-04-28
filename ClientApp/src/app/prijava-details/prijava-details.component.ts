import { Component, Inject, OnInit, ViewChild, ElementRef, ViewChildren, QueryList } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { jsonIgnore } from 'json-ignore';
import { NgbActiveModal, NgbTypeahead, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, catchError, map } from 'rxjs/operators';
import { NgbdModalConfirm } from '../modal-focus/modal-focus.component';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import { environment } from '../../environments/environment';
import { AuthenticationService } from '../auth/auth.service';
import { Prijava } from '../model/prijava';
import { partner } from '../model/partner';
import { Korisnik } from '../model/korisnik';
import { FormMode } from '../enums/formMode';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { UrlResolver } from '@angular/compiler';
import { Artikal } from '../model/artikal';

@Component({
  selector: 'app-prijava-details',
  templateUrl: './prijava-details.component.html'
})


export class PrijavaDetailsComponent {
  public model: any;
  public formMode: FormMode;
  currentUser: Korisnik;
  stavkeVisible: boolean = true;
  prijavaVisible: boolean = true;
  dokumentiVisible: boolean = true;
  isExpanded = false;
  public selectedPrijava: Prijava;
  dokumentForm: FormGroup;

  @ViewChild("partnerElement", { static: false }) partnerElement: ElementRef;
  @ViewChildren("naziv") naziv: QueryList<ElementRef>;
  selectedArtikal: Artikal;

  constructor(private _sanitizer: DomSanitizer,
    private formBuilder: FormBuilder, private authenticationService: AuthenticationService, private modalService: NgbModal, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public activeModal: NgbActiveModal, private toastr: ToastrService) {
    toastr.toastrConfig.positionClass = "toast-bottom-right";
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
    this.formMode = FormMode.View;
    this.dokumentForm = this.formBuilder.group({
      dokument: ''
    });
  }


  public startAdd() {
    this.formMode = FormMode.Add;
    this.prijavaVisible = true;
    this.selectedPrijava = new Prijava();
    this.selectedPrijava.datum = (new Date());
    setTimeout(_ => {
      this.partnerElement.nativeElement.focus();
    }, 0);
  }
  savePrijava(prijava) {

    prijava.garantni_rok = this.convertToNumber(prijava.garantni_rok);
    if ((typeof prijava.partner) == "string") {
      prijava.partner_naziv = prijava.partner;
      prijava.partner = new partner();
      prijava.partner.naziv = prijava.partner_naziv;
    }
    if (prijava.broj == undefined) {
      let obj: object = prijava.datum;
      this.http.post<Prijava>(this.baseUrl + 'prijava', prijava).subscribe(result => {
        console.log("OK");
        this.toastr.success("Prijava je uspješno dodata..");
        this.formMode = FormMode.View;
        prijava = result;
        this.selectedPrijava = prijava;
       
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    } else {
      this.http.put<Prijava>(this.baseUrl + 'prijava', prijava).subscribe(result => {
        console.log("OK");
        this.toastr.success("Prijava je uspješno sačuvana..");
        this.formMode = FormMode.View;
        prijava = result;
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    }
  }
  izmijeniPrijavu() {
    this.formMode = FormMode.Edit;
    setTimeout(_ => {
      this.partnerElement.nativeElement.focus();
    }, 0);
  }
  getStatus(status) {
    if (status == "E")
      return "Evidentirana";
    else if (status == "Z")
      return "Zaključena";
    else if (status == "R")
      return "Realizovana";
    else if (status == "D")
      return "Djelimično realizovana";
    else if (status == "N")
      return "Nerealizovana";
  }
  collapse() {
    this.isExpanded = false;
  }
  toggle() {
    this.isExpanded = !this.isExpanded;
  }
 
  
 
  obrisiPrijavu() {
    if (this.selectedPrijava != undefined) {
      let modalRef = this.modalService.open(NgbdModalConfirm);
      modalRef.result.then((data) => {
        this.http.delete(this.baseUrl + 'prijava/obrisiPrijavu?broj=' + this.selectedPrijava.broj).subscribe(result => {
          this.toastr.success("Prijava je uspješno obrisana..");
          this.activeModal.close();
        }, error => {
          this.toastr.error("Greška..");
          console.error(error)
        });
      }, (reason) => {
      });

      modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati prijavu " + this.selectedPrijava.broj + " ?";
    }
  }

  selectedItem(item) {
    this.selectedPrijava.kupac_sifra = item["item"]["sifra"];
    this.selectedPrijava.kupac_ime = item["item"]["naziv"];
    this.selectedPrijava.kupac_adresa = item["item"]["adresa"];
    this.selectedPrijava.kupac_email = item["item"]["email"];
    this.selectedPrijava.kupac_telefon = item["item"]["telefon"];
  }
  downLoadFile(data: any, type: string) {
    let blob = new Blob([data], { type: type });
    let url = window.URL.createObjectURL(blob);
    let pwa = window.open(url);
    if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
      alert('Please disable your Pop-up blocker and try again.');
    }
  }
  excel() {
    if (this.selectedPrijava != undefined) {

      this.http.get(this.baseUrl + 'prijava/excel?broj=' + this.selectedPrijava.broj
        , {
          responseType: 'arraybuffer'
        }
      ).subscribe(response => this.downLoadFile(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
    }
  }
  radniNalog() {
    if (this.selectedPrijava != undefined) {

      this.http.get(this.baseUrl + 'prijava/radniNalog?broj=' + this.selectedPrijava.broj
        , {
          responseType: 'arraybuffer'
        }
      ).subscribe(response => this.downLoadFile(response, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"));
    }
  }
  selectedItemDobavljac(item) {
    this.selectedPrijava.dobavljac_sifra = item["item"]["sifra"];
    this.selectedPrijava.dobavljac= item["item"]["naziv"];
  }
  cancel() {
    if (this.formMode == FormMode.Add || this.formMode == FormMode.Edit) {
      let modalRef = this.modalService.open(NgbdModalConfirm);
      modalRef.result.then((data) => {
        this.activeModal.close();
      }, (reason) => {
      });
      modalRef.componentInstance.confirmText = "Da li ste sigurni da želite zatvoriti prikaz? Sve nesačuvane izmjene će biti poništene.";
    }
    else
      this.activeModal.close();
  }
  search = (text$: Observable<string>) => {
    return text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      // switchMap allows returning an observable rather than maps array
      switchMap((searchText) => this.searchPartner(searchText)),
      catchError(null)
    );
  }
  resultFormatPartnerListValue(value: any) {
    return value.sifra + "-" + value.naziv;
  }
  inputFormatPartnerListValue(value: any) {
    if (value.naziv)
      return value.naziv
    return value;

  }

  searchPartner(term: string) {
    if (term === '') {
      return of([]);
    }
    return this.http.get(this.baseUrl + 'partner/search?naziv=' + term)
      .pipe(
        map((response) => {
          console.log(response);
          return response;
        })
      );
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





}




