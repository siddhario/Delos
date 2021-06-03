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
import { partner } from '../model/partner';
import { Korisnik } from '../model/korisnik';
import { FormMode } from '../enums/formMode';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { UrlResolver } from '@angular/compiler';
import { Artikal } from '../model/artikal';
import { Ugovor } from '../model/ugovor';
import { UgovorRata } from '../model/ugovorRata';
import { PregledUplataComponent } from '../pregledUplata/pregledUplata.component';

import * as _ from 'lodash'

@Component({
  selector: 'app-ugovor-details',
  templateUrl: './ugovor-details.component.html'
})


export class UgovorDetailsComponent {
  public model: any;
  public formMode: FormMode;
  currentUser: Korisnik;
  stavkeVisible: boolean = true;
  ugovorVisible: boolean = true;
  dokumentiVisible: boolean = true;
  isExpanded = false;
  public selectedUgovor: Ugovor;
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
    this.ugovorVisible = true;
    this.selectedUgovor = new Ugovor();

    this.selectedUgovor.status = "E";
    this.selectedUgovor.inicijalno_placeno = 0;
    this.selectedUgovor.datum = (new Date());
    setTimeout(_ => {
      this.partnerElement.nativeElement.focus();
    }, 0);
  }

  convertProperties(ugovor: Ugovor) {
    ugovor.iznos_sa_pdv = this.convertToNumber(ugovor.iznos_sa_pdv);
    ugovor.broj_rata = this.convertToNumber(ugovor.broj_rata);
    ugovor.inicijalno_placeno = this.convertToNumber(ugovor.inicijalno_placeno);
    ugovor.preostalo_za_uplatu = this.convertToNumber(ugovor.preostalo_za_uplatu);
    ugovor.suma_uplata = this.convertToNumber(ugovor.suma_uplata);
    ugovor.uplaceno_po_ratama = this.convertToNumber(ugovor.uplaceno_po_ratama);
    for (let i = 0; i < ugovor.rate.length; i++) {
      ugovor.rate[i].uplaceno = this.convertToNumber(ugovor.rate[i].uplaceno);
    }

  }
  potvrda(rata: UgovorRata) {
    this.http.get(this.baseUrl + 'ugovor/potvrda?broj=' + rata.ugovorbroj + "&broj_rate=" + rata.broj_rate
      , {
        responseType: 'arraybuffer'
      }
    ).subscribe(response => this.downLoadFile(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
  }

  saveUgovorRata(rata: UgovorRata) {
    rata.formMode = FormMode.View;
    rata.uplaceno = this.convertToNumber(rata.uplaceno);
    this.http.put<UgovorRata>(this.baseUrl + 'ugovor/updateRate', rata).subscribe(result => {
      console.log("OK");
      this.toastr.success("Uspješno...");

      this.selectedUgovor = result;
      let sorted:UgovorRata[]=[];
      this.selectedUgovor.rate = this.selectedUgovor.rate.sort((a,b)=>(a.broj_rate > b.broj_rate) ? 1 : ((b.broj_rate > a.broj_rate) ? -1 : 0));
 
        let modalRef = this.modalService.open(NgbdModalConfirm);
        modalRef.result.then((data) => {

          this.http.get(this.baseUrl + 'ugovor/potvrda?broj=' + rata.ugovorbroj + "&broj_rate=" + rata.broj_rate
            , {
              responseType: 'arraybuffer'
            }
          ).subscribe(response => this.downLoadFile(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }, (reason) => {
        });

        modalRef.componentInstance.confirmText = "Štampati potvrdu ?";
      
    }, error => {
      this.toastr.error("Greška..");
      console.error(error)
    });
  }

  saveUgovor(ugovor) {
    if (ugovor.datum == "")
      ugovor.datum = null;
    if (ugovor.odobrio == undefined) {
      this.toastr.error("Polje 'Odobrio' je obavezno za unos!");
      return;
    }
    if (ugovor.iznos_sa_pdv == undefined) {
      this.toastr.error("Polje 'Iznos' je obavezno za unos!");
      return;
    }
    if (ugovor.broj_rata == undefined) {
      this.toastr.error("Polje 'Broj rata' je obavezno za unos!");
      return;
    }
    if (ugovor.inicijalno_placeno == undefined) {
      this.toastr.error("Polje 'Inicijalno uplaćeno' je obavezno za unos!");
      return;
    }
    this.convertProperties(ugovor);
    if ((typeof ugovor.partner) == "string") {
      ugovor.kupac_naziv = ugovor.partner;
      ugovor.partner = new partner();
      ugovor.partner.naziv = ugovor.kupac_naziv;
    }

    if (this.rate != null)
      ugovor.rate = this.rate;
    if (ugovor.broj == undefined) {
      let obj: object = ugovor.datum;
      this.http.post<Ugovor>(this.baseUrl + 'ugovor', ugovor).subscribe(result => {
        console.log("OK");
        this.toastr.success("Ugovor je uspješno dodat..");
        this.formMode = FormMode.View;
        ugovor = result;
        this.selectedUgovor = ugovor;

      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    } else {
      this.http.put<Ugovor>(this.baseUrl + 'ugovor', ugovor).subscribe(result => {
        console.log("OK");
        this.toastr.success("Ugovor je uspješno sačuvan..");
        this.formMode = FormMode.View;
        ugovor = result;
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    }
  }
  preuzmiIznos(ugovorRata: UgovorRata) {
    ugovorRata.formMode = 3;
    ugovorRata.uplaceno = ugovorRata.iznos;
    ugovorRata.datum_placanja = new Date();
  }
  izmijeniUgovor() {
    this.formMode = FormMode.Edit;
    setTimeout(_ => {
      this.partnerElement.nativeElement.focus();
    }, 0);
  }
  getStatus(status) {
    if (status == "E")
      return "Evidentiran";
    else if (status == "Z")
      return "Zaključen";
    else if (status == "R")
      return "Realizovan";
  }
  collapse() {
    this.isExpanded = false;
  }
  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  zakljuciUgovor() {
    if (this.selectedUgovor != undefined) {
      let modalRef = this.modalService.open(NgbdModalConfirm);
      modalRef.result.then((data) => {
        this.http.get(this.baseUrl + 'ugovor/zakljuci?broj=' + this.selectedUgovor.broj).subscribe(result => {
          this.toastr.success("Ugovor je uspješno zaključen..");
          this.activeModal.close();


          let modalRef = this.modalService.open(NgbdModalConfirm);
          modalRef.result.then((data) => {
            this.http.get(this.baseUrl + 'ugovor/excel?broj=' + this.selectedUgovor.broj
              , {
                responseType: 'arraybuffer'
              }
            ).subscribe(response => this.downLoadFile(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
          }, (reason) => {
          });

          modalRef.componentInstance.confirmText = "Štampati ugovor " + this.selectedUgovor.broj + " ?";

        }, error => {
          this.toastr.error("Greška..");
          console.error(error)
        });
      }, (reason) => {
      });

      modalRef.componentInstance.confirmText = "Da li ste sigurni da želite zaključiti ugovor " + this.selectedUgovor.broj + " ?";
    }
  }

  otkljucajUgovor() {
    if (this.selectedUgovor != undefined) {
      let modalRef = this.modalService.open(NgbdModalConfirm);
      modalRef.result.then((data) => {
        this.http.get(this.baseUrl + 'ugovor/otkljucaj?broj=' + this.selectedUgovor.broj).subscribe(result => {
          this.toastr.success("Ugovor je uspješno otključan..");
          this.activeModal.close();
        }, error => {
          this.toastr.error("Greška..");
          console.error(error)
        });
      }, (reason) => {
      });

      modalRef.componentInstance.confirmText = "Da li ste sigurni da želite otključati ugovor " + this.selectedUgovor.broj + " ?";
    }
  }

  ugovorRealizovan() {
    if (this.selectedUgovor != undefined) {
      let modalRef = this.modalService.open(NgbdModalConfirm);
      modalRef.result.then((data) => {
        this.http.get(this.baseUrl + 'ugovor/realizovan?broj=' + this.selectedUgovor.broj).subscribe(result => {
          this.toastr.success("Ugovor je uspješno proglašen realizovanim..");
          this.activeModal.close();
        }, error => {
          this.toastr.error("Greška..");
          console.error(error)
        });
      }, (reason) => {
      });

      modalRef.componentInstance.confirmText = "Da li ste sigurni da želite proglasiti realizovanim ugovor " + this.selectedUgovor.broj + " ?";
    }
  }

  obrisiUgovor() {
    if (this.selectedUgovor != undefined) {
      let modalRef = this.modalService.open(NgbdModalConfirm);
      modalRef.result.then((data) => {
        this.http.delete(this.baseUrl + 'ugovor/obrisiUgovor?broj=' + this.selectedUgovor.broj).subscribe(result => {
          this.toastr.success("Ugovor je uspješno obrisana..");
          this.activeModal.close();
        }, error => {
          this.toastr.error("Greška..");
          console.error(error)
        });
      }, (reason) => {
      });

      modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati ugovor " + this.selectedUgovor.broj + " ?";
    }
  }

  selectedItem(item) {
    this.selectedUgovor.kupac_sifra = item["item"]["sifra"];
    this.selectedUgovor.kupac_naziv = item["item"]["naziv"];
    this.selectedUgovor.kupac_adresa = item["item"]["adresa"];
    this.selectedUgovor.kupac_telefon = item["item"]["telefon"];
    this.selectedUgovor.kupac_broj_lk = item["item"]["broj_lk"];
    this.selectedUgovor.kupac_maticni_broj = item["item"]["maticni_broj"];
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
    if (this.selectedUgovor != undefined) {

      this.http.get(this.baseUrl + 'ugovor/excel?broj=' + this.selectedUgovor.broj
        , {
          responseType: 'arraybuffer'
        }
      ).subscribe(response => this.downLoadFile(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
    }
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
  brojRata: number;
  iznos: number;
  inicijalnoUplaceno: number;
  iznosRate: number;

  KreirajRateUgovora() {

    this.rate = new Array<UgovorRata>();
    if (this.selectedUgovor != null && this.selectedUgovor.status == "E") {

      this.brojRata = this.convertToNumber(this.selectedUgovor.broj_rata);
      this.iznos = this.convertToNumber(this.selectedUgovor.iznos_sa_pdv);
      this.inicijalnoUplaceno = this.convertToNumber(this.selectedUgovor.inicijalno_placeno);
      if (this.brojRata > 0) {
        this.iznosRate = +((this.iznos - this.inicijalnoUplaceno) / this.brojRata).toFixed(2);
        let sumaIznosaRata = 0;
        let r: UgovorRata;
        let counter
        for (let i = 0; i < this.brojRata - 1; i++) {
          r = new UgovorRata();
          r.ugovorbroj = this.selectedUgovor.broj;
          r.broj_rate = i + 1;
          r.datum_placanja = null;
          r.uplaceno = 0;
          r.iznos = this.iznosRate;
          r.rok_placanja = new Date((new Date()).setMonth((new Date()).getMonth() + i + 1));
          this.rate.push(r);
          sumaIznosaRata = sumaIznosaRata + this.iznosRate;
        }
        r = new UgovorRata();
        r.ugovorbroj = this.selectedUgovor.broj;
        r.broj_rate = this.brojRata;
        r.datum_placanja = null;
        r.uplaceno = 0;
        r.iznos = +((this.iznos - this.inicijalnoUplaceno) - sumaIznosaRata).toFixed(2);
        r.rok_placanja = new Date((new Date()).setMonth((new Date()).getMonth() + this.brojRata));
        this.rate.push(r);
        this.selectedUgovor.rate = this.rate;
      }
    }
  }

  rate: UgovorRata[];

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


