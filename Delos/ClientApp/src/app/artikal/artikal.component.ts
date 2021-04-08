import { Component, Inject, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Artikal } from '../model/artikal';
import { Korisnik } from '../model/korisnik';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { fromEvent, Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError, map, filter } from 'rxjs/operators';
import { DomSanitizer } from '@angular/platform-browser';
import { ExcelService } from '../../services/export-excel-service';
import { Kategorija } from '../model/kategorija';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgbdModalConfirm } from '../modal-focus/modal-focus.component';
import { ToastrService } from 'ngx-toastr';
import { istorija_cijena } from '../model/artikal - Copy';
import { QueryResult } from '../model/queryResult';
import * as XLSX from 'xlsx';
import isImageURL from 'is-image-url';

@Component({
  selector: 'app-korisnik',
  templateUrl: './artikal.component.html'
})
export class ArtikalComponent {
  public artikli: Artikal[] = [];
  sortOrder: boolean;
  sortColumn: string;
  searchText: string;
  loadAll: string = "0";
  currentUser: Korisnik;
  kategorije: Array<string> = new Array<string>();
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
  selectedBrend: string;
  distinctBrendovi: Set<any>;
  brendovi: string[];
  selectedAktivan: string = "1";
  selectedKategorijeFilter = [];
  page: number = 1;
  pageCount: number = 1;
  loading: boolean;
  //pages: Array<number>=[];
  //getPageActiveClass(page) {
  //  if (page == this.page) {
  //    return "page-item active";
  //  }
  //  else
  //    return "page-item";
  //}
  file: File;
  arrayBuffer: any;
  filelist: any;
  importFromXLSX(event) {
    this.file = event.target.files[0];
    let fileReader = new FileReader();
    fileReader.readAsArrayBuffer(this.file);
    fileReader.onload = (e) => {
      this.arrayBuffer = fileReader.result;
      var data = new Uint8Array(this.arrayBuffer);
      var arr = new Array();
      for (var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);
      var bstr = arr.join("");
      var workbook = XLSX.read(bstr, { type: "binary" });
      var first_sheet_name = workbook.SheetNames[0];
      var worksheet = workbook.Sheets[first_sheet_name];
      console.log(XLSX.utils.sheet_to_json(worksheet, { raw: true }));
      var arraylist = XLSX.utils.sheet_to_json(worksheet, { raw: true });
      //this.filelist = [];
      //console.log(this.filelist)
      let artikliImport = new Array<Artikal>();
      for (let i = 0; i < arraylist.length; i++) {
        let row = arraylist[i];
        let artikal = new Artikal();
        artikal.dobavljac_sifra = row["SIFRA"].toString();
        artikal.dobavljac = row["DOBAVLJAC"].toString();
        artikal.cijena_mp = row["MPC"];
        artikal.cijena_prodajna = row["VPC"];
        artikal.naziv = row["NAZIV"].toString();
        artikal.opis = row["OPIS"].toString();
        artikal.cijena_sa_rabatom = row["NC"];
        artikal.kategorija = row["KATEGORIJA"].toString();
        artikal.barkod = row["BARKOD"].toString();
        artikal.brend = row["BREND"].toString();
        let photos: string = row["URL_PHOTO"];
        if (photos != undefined)
          artikal.slike = photos.split('\n');
        artikal.kategorija = row["KATEGORIJA"].toString();
        artikal.kolicina = row["KOLICINA"];

        artikliImport.push(artikal);

      }
      if (artikliImport.length > 0) {
        let h = new HttpHeaders();
        h.append('Content-Type', 'application/json');
        this.http.post<Array<Artikal>>(this.baseUrl + 'webShopSync/artikliImport', artikliImport, { headers: h }).subscribe(result => {

          this.toastr.success("Import uspješan!");
          //this.dokumenti.load();
        }, error => {
          this.toastr.error("Greška..");
          console.error(error)
        })
      }
      else {
        this.toastr.info("Nema podataka za import!");
      }
    }
  }
  url: string;
  addUrl(artikal: Artikal) {
    let isurl = isImageURL(this.url);

    if (isurl == true) {
      this.http.post(this.baseUrl + 'webShopSync/addPhotoURL?sifraArtikla=' + artikal.sifra + '&url=' + this.url, null).subscribe(result => {
        console.log("OK");
        if (artikal.slike == null)
          artikal.slike = [];
        artikal.slike.push(this.url);

        this.url = null;
        this.toastr.success("URL je uspješno dodat..");
        this.activeModal.close("OK");
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    }
    else {
      this.toastr.error("URL nije prepoznat kao validan url slike!");
    }


  }

  deletePhotoURL(artikal: Artikal, url: string) {
    let modalRef = this.modalService.open(NgbdModalConfirm);
    modalRef.result.then((data) => {
      this.http.delete(this.baseUrl + 'webShopSync/deletePhotoURL?sifraArtikla=' + artikal.sifra + '&url=' + url).subscribe(result => {
        console.log("OK");
        this.toastr.success("Slika artikla je uspješno obrisana..");
        this.search();
        this.activeModal.close("DELETE");
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    }, (reason) => {
    });

    modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati sliku \"" + url + "\"?";
  }

  delete(artikal: Artikal) {
    let modalRef = this.modalService.open(NgbdModalConfirm);
    modalRef.result.then((data) => {
      this.http.delete(this.baseUrl + 'webShopSync/delete?sifra=' + artikal.sifra).subscribe(result => {
        console.log("OK");
        this.toastr.success("Artikal je uspješno obrisan..");
        this.search();
        this.activeModal.close("DELETE");
      }, error => {
        this.toastr.error("Greška..");
        console.error(error)
      });
    }, (reason) => {
    });

    modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati artikal \"" + artikal.sifra + "\"?";
  }
  //public importFromFile(bstr: string): XLSX.AOA2SheetOpts {
  //  /* read workbook */
  //  const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

  //  /* grab first sheet */
  //  const wsname: string = wb.SheetNames[0];
  //  const ws: XLSX.WorkSheet = wb.Sheets[wsname];

  //  /* save data */
  //  const data = <XLSX.AOA2SheetOpts>(XLSX.utils.sheet_to_json(ws, { header: 1 }));

  //  return data;
  //}

  startSearch(naziv: string, selectedKategorijaWebShop: string, dostupnost: string, dobavljac: string, loadAll: string, brend: string, aktivan: string, page: number) {
    this.selectedKategorijaWebShop = selectedKategorijaWebShop;
    this.loadAll = loadAll;
    this.selectedDostupnost = dostupnost;
    //this.selectedKategorija = kategorija;
    this.selectedDobavljac = dobavljac;
    this.page = page;
    this.selectedBrend = brend;
    this.selectedAktivan = aktivan;

    this.isSearching = true;
    if (selectedKategorijaWebShop == "--Sve--")
      selectedKategorijaWebShop = null;
    if (selectedKategorijaWebShop == "--Nekategorisano--")
      selectedKategorijaWebShop = "NULL";
    if (dobavljac == "--Svi--")
      dobavljac = null;
    if (brend == "--Svi--")
      brend = null;
    if (aktivan == "--Svi--")
      aktivan = null;
    this.loading = true;

    this.http.get<QueryResult>(this.baseUrl + 'webShopSync/artikliSearch?'
      + (naziv ? 'naziv=' + naziv : '')
      + (selectedKategorijaWebShop ? '&kategorija=' + selectedKategorijaWebShop : '')
      + (dostupnost ? '&dostupnost=' + dostupnost : '')
      + (dobavljac ? '&dobavljac=' + dobavljac : '')
      + (loadAll ? '&loadAll=' + loadAll : '')
      + (brend ? '&brend=' + brend : '')
      + (aktivan ? '&aktivan=' + aktivan : '')
      + ('&page=' + this.page)
      + ('&pageSize=50')
    ).subscribe(result => {
      this.loading = false;
      // this.pages = [];

      //for (let i = 0; i < result.pageCount; i++) {
      //  this.pages.push(i+1);
      //}
      if (result.data != null)
        this.artikli = this.artikli.concat(result.data);
      this.pageCount = result.pageCount;

      this.isSearching = false;

      this.selectedKategorijeFilter = new Array<string>();
      this.selectedKategorijeFilter.push("--Svi--");

      this.artikli.forEach(a => {
        this.selectedKategorijeFilter.push(a.kategorija);
      });
      let katgs = []
      katgs = this.selectedKategorijeFilter.sort((a, b) => {
        if (a < b)
          return -1;
        else if (a > b)
          return 1;
        else
          return 0;
      })

      this.distinctKategorije = new Set(katgs);


      if (brend == null) {
        this.brendovi = new Array<string>();
        this.brendovi.push("--Svi--");

        this.artikli.forEach(a => {
          this.brendovi.push(a.brend);
        });
        let brnds = []
        brnds = this.brendovi.sort((a, b) => {
          if (a < b)
            return -1;
          else if (a > b)
            return 1;
          else
            return 0;
        })

        this.distinctBrendovi = new Set(brnds);
      }
    }, error => { console.error(error); this.loading = false; });
  }
  selectedItem: Artikal;
  selectItem(artikal: Artikal) {
    if (this.selectedItem == artikal)
      this.selectedItem = null;
    else
      this.selectedItem = artikal;
  }
  sanitize(url: string) {
    return this.sanitizer.bypassSecurityTrustUrl(url);
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
  //    this.startSearch(text, this.selectedKategorijaWebShop, this.selectedDostupnost, this.selectedDobavljac, this.loadAll, this.selectedBrend, this.selectedAktivan, 1);
  //  });


  //}
  selectedDobavljac: string;


  sortProperty(property) {
    this.sortColumn = property;
    if (property == "sifra")
      this.sort(p => p.sifra, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "barkod")
      this.sort(p => p.barkod, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "aktivan")
      this.sort(p => p.aktivan, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "naziv")
      this.sort(p => p.naziv, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "kategorija")
      this.sort(p => p.kategorija, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "dobavljac")
      this.sort(p => p.dobavljac, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "brend")
      this.sort(p => p.brend, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "cijena_sa_rabatom")
      this.sort(p => p.cijena_sa_rabatom, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "cijena_prodajna")
      this.sort(p => p.cijena_prodajna, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "cijena_mp")
      this.sort(p => p.cijena_mp, this.sortOrder == true ? "ASC" : "DESC");
    else if (property == "dostupnost")
      this.sort(p => p.dostupnost, this.sortOrder == true ? "ASC" : "DESC");

  }
  sort<T>(prop: (c: Artikal) => T, order: "ASC" | "DESC"): void {
    this.artikli.sort((a, b) => {
      if (prop(a) < prop(b) || prop(a) == null)
        return -1;
      if (prop(a) > prop(b) || prop(b) == null)
        return 1;
      return 0;
    });

    if (order === "DESC") {
      this.artikli.reverse();
      this.sortOrder = true;
    } else {
      this.sortOrder = false;
    }
  }

  sortIstorija(istorija: Array<istorija_cijena>): Array<istorija_cijena> {
    istorija.sort((a, b) => {
      if (a.vrijeme < b.vrijeme)
        return 1;
      else
        return -1;
    });
    return istorija;
  }

  toggleAktivan(artikal: Artikal) {
    let modalRef = this.modalService.open(NgbdModalConfirm);
    modalRef.result.then((data) => {
      artikal.aktivan = !artikal.aktivan;
      this.http.put<Kategorija>("webShopSync/updateArtikal", artikal).subscribe(result => {
        this.toastr.success("Artikal je uspješno izmjenjen..");
      }, error => console.error(error));
    });

    modalRef.componentInstance.confirmText = "Da li ste sigurni da želite " + (artikal.aktivan == true ? "deaktivirati" : "aktivirati") + " artikal  " + artikal.naziv + " ? ";


  }
  exportAsXLSX(): void {
    this.excelService.exportAsExcelFile(this.artikli, 'Artikli');
  }

  hasRole(rola: string) {
    return this.currentUser.role.includes(rola);
  }
  search() {
    this.page = 1;
    this.artikli = [];
    console.log(this.searchText);
    this.startSearch(this.searchText, this.selectedKategorijaWebShop, this.selectedDostupnost, this.selectedDobavljac, this.loadAll, this.selectedBrend, this.selectedAktivan, 1);
  }
  onScroll() {
    if (this.page < this.pageCount) {
      this.page = this.page + 1;
      this.startSearch(this.searchText, this.selectedKategorijaWebShop, this.selectedDostupnost, this.selectedDobavljac, this.loadAll, this.selectedBrend, this.selectedAktivan, 1);

    }
  }
  constructor(private modalService: NgbModal, private toastr: ToastrService, private excelService: ExcelService, private sanitizer: DomSanitizer, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private authenticationService: AuthenticationService, private router: Router, public activeModal: NgbActiveModal) {
    this.baseUrl = baseUrl;
    this.http = http;
    this.sortColumn = 'naziv';
    this.sortOrder = true;
    this.selectedDobavljac = "--Svi--";
    this.selectedKategorijaWebShop = "--Sve--";
    this.authenticationService.currentUser.subscribe(x => {
      this.currentUser = x;
      if (this.currentUser == null || (this.currentUser.role.includes('ADMIN') == false && this.currentUser.role.includes("WEBSHOP") == false && this.currentUser.role.includes("WEBSHOP_ADMIN") == false))
        this.router.navigate(['/login']);
    });

    http.get<Kategorija[]>(baseUrl + 'webShopSync/kategorije').subscribe(result => {
      this.kategorijeWebShop = [{ naziv: "--Sve--" }, { naziv: "--Nekategorisano--" }];
      this.kategorijeWebShop = this.kategorijeWebShop.concat(result);
    }, error => console.error(error));

  }
}

