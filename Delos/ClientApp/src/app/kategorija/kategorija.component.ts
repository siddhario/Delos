import { Component, Inject, ViewChild, ElementRef, OnInit, AfterViewInit, ViewChildren, QueryList } from '@angular/core';
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
import { NgbdModalConfirm } from '../modal-focus/modal-focus.component';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-kategorija',
  templateUrl: './kategorija.component.html'
})
export class KategorijaComponent {
  public kategorije: Kategorija[];
  sortOrder: boolean;
  sortColumn: string;
  searchText: string;
  currentUser: Korisnik;
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


  selectedItem: Kategorija;
  selectItem(kategorija: Kategorija) {
    if (this.selectedItem == kategorija)
      this.selectedItem = null;
    else
      this.selectedItem = kategorija;
  }
  sanitize(url: string) {
    return this.sanitizer.bypassSecurityTrustUrl(url);
  }

  selectedDobavljac: string;

  save(selectedItem: Kategorija) {
    selectedItem.marza = this.convertToNumber(selectedItem.marza);
    this.http.put<Kategorija>("webShopSync/updateKategorija", selectedItem).subscribe(result => {
      this.toastr.success("Web shop kategorija je uspješno izmjenjena..");
    }, error => console.error(error));
  }


  sortProperty(property) {
    this.sortColumn = property;
    if (property == "sifra")
      this.sort(p => p.sifra, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "naziv")
      this.sort(p => p.naziv, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "marza")
      this.sort(p => p.marza, this.sortOrder == true ? "ASC" : "DESC");


  }
  expand: boolean = false;
  toggleExpand() {
    this.expand = !this.expand;
  }
  removeKategorija(vrsta) {

    let modalRef = this.modalService.open(NgbdModalConfirm);
    modalRef.result.then((data) => {

      this.selectedItem.kategorije_dobavljaca.splice(this.selectedItem.kategorije_dobavljaca.indexOf(vrsta), 1);

      this.save(this.selectedItem);
    });

    modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati kategoriju dobavljača " + vrsta + " iz kategorije " + this.selectedItem.naziv+" ? ";
  }
  @ViewChildren("novaKategorija") novaKategorija: QueryList<ElementRef<any>>;

  novaWebShopKategorija: string;
  addWebShopKategorija(kategorija) {
    let kat = new Kategorija();
    kat.naziv = kategorija;
    kat.kategorije_dobavljaca = [];
    this.http.post<Kategorija>("webShopSync/insertKategorija", kat).subscribe(result => {
      this.toastr.success("Web shop kategorija je uspješno dodana..");
      this.loadAll();
    },
      error => console.error(error));
  }
  loadAll() {
    this.http.get<Kategorija[]>(this.baseUrl + 'webShopSync/kategorije').subscribe(result => {
      this.kategorije = result;
    }, error => console.error(error));
  }

  toggleAktivna(kategorija: Kategorija) {
    let modalRef = this.modalService.open(NgbdModalConfirm);
    modalRef.result.then((data) => {
      kategorija.aktivna = !kategorija.aktivna;
      this.save(kategorija);
    });

    modalRef.componentInstance.confirmText = "Da li ste sigurni da želite " +( kategorija.aktivna==true?"deaktivirati":"aktivirati") + " kategoriju  " + kategorija.naziv + " ? ";
  }

  changeKategorija(kategorija, vrsta) {
  }

  //novaKategorija: string;
  addKategorija(kat, vrsta) {

    let kategorija = this.kategorije.find(k => k.naziv == kat);
    if (vrsta == null) {
      vrsta = this.novaKategorija.find(s => s.nativeElement.id == "nk_" + kategorija.sifra).nativeElement.value;
      vrsta = vrsta.trim();
    }


    let uKategorijama = this.kategorije.filter(k => k.kategorije_dobavljaca.includes(vrsta));
    if (uKategorijama.length == 1 && uKategorijama.includes(kategorija))
      return;
    if (uKategorijama && uKategorijama.length > 0) {
      let modalRef = this.modalService.open(NgbdModalConfirm);
      modalRef.result.then((data) => {
        uKategorijama.forEach(k => {
          k.kategorije_dobavljaca.splice(k.kategorije_dobavljaca.indexOf(vrsta), 1);
        });

        if (kategorija.kategorije_dobavljaca.includes(vrsta) == false)
          kategorija.kategorije_dobavljaca.push(vrsta);

        this.save(kategorija);
      }, (reason) => {
        //  if (kategorija.kategorije_dobavljaca.includes(vrsta) == false) {
        //    kategorija.kategorije_dobavljaca.push(vrsta);
        //    this.save(kategorija);
        //}
      });
      let nazivi = uKategorijama.map(k => {
        return k.naziv;
      }).join("\n,");
      modalRef.componentInstance.confirmText = "Ova kategorija dobavljača pripada web shop kategorijama \n" + nazivi + ". Da li želite da je obrišete iz istih i dodate u kategoriju " + kategorija.naziv + "?";
    }
    else {
      if (kategorija.kategorije_dobavljaca.includes(vrsta) == false) {
        kategorija.kategorije_dobavljaca.push(vrsta);
        this.save(kategorija);
      }

    }

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
  sort<T>(prop: (c: Kategorija) => T, order: "ASC" | "DESC"): void {
    this.kategorije.sort((a, b) => {
      if (prop(a) < prop(b) || prop(a) == null)
        return -1;
      if (prop(a) > prop(b) || prop(b) == null)
        return 1;
      return 0;
    });

    if (order === "DESC") {
      this.kategorije.reverse();
      this.sortOrder = true;
    } else {
      this.sortOrder = false;
    }
  }
  updateInProgress: boolean = false;
  azurirajKategorije() {
    this.updateInProgress = true;
    this.http.get(this.baseUrl + 'webShopSync/updateKategorije').subscribe(result => {
      this.toastr.success("Kategorije su uspješno ažurirane..");
      this.updateInProgress = false;
    }, error => { this.updateInProgress = false; console.error(error) });
  }

  obrisiKategoriju(kategorija) {
    let modalRef = this.modalService.open(NgbdModalConfirm);
    modalRef.result.then((data) => {
      this.http.delete(this.baseUrl + 'webShopSync/deleteKategorija?sifra=' + kategorija.sifra).subscribe(result => {
        this.toastr.success("Kategorija je uspješno obrisana..");
        this.kategorije.splice(this.kategorije.indexOf(kategorija), 1);
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    }, (reason) => {
    });

    modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati kategoriju " + kategorija.naziv + " ?";


  }
  exportAsXLSX(): void {
    this.excelService.exportAsExcelFile(this.kategorije, 'Kategorije');
  }
  constructor(private modalService: NgbModal, private excelService: ExcelService, private sanitizer: DomSanitizer, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private authenticationService: AuthenticationService, private router: Router, private toastr: ToastrService) {
    this.baseUrl = baseUrl;
    this.http = http;
    this.sortColumn = 'naziv';
    this.sortOrder = true;
    this.selectedDobavljac = "--Svi--";
    this.selectedKategorijaWebShop = "--Sve--";
    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;
      if (this.currentUser == null)
        this.router.navigate(['/login']);
    });

    this.loadAll();

  }
}

