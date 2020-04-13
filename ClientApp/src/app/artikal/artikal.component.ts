import { Component, Inject, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Artikal } from '../model/artikal';
import { Korisnik } from '../model/korisnik';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { fromEvent, Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError, map, filter } from 'rxjs/operators';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'app-korisnik',
    templateUrl: './artikal.component.html'
})
export class ArtikalComponent implements AfterViewInit {
    public artikli: Artikal[];
    sortOrder: boolean;
    sortColumn: string;
    searchText: string;
    currentUser: Korisnik;
    kategorije: Array<string> = new Array<string>();
    selectedKategorija: string;
    distinctKategorije: Set<string>;
    @ViewChild('searchInput', { static: false }) searchInput: ElementRef<any>;

    dobavljaci = ["--Svi--","ASBIS", "AVTERA", "COMTRADE", "KIMTEC", "MINT", "UNIEXPERT"];
    artikliOriginal: Array<Artikal> = [];
    isSearching: boolean;
    selectedDostupnost: string = "0";
    searchArtikli(searchText: string, kategorija: string, dostupnost: string,dobavljac:string): Observable<any> {
        if (!searchText || this.searchText.length < 3)
            return of([]);
        if (kategorija != null)
            kategorija = kategorija.substring(kategorija.indexOf("] ") + 2);
        let words = this.searchText.split(" ");
        let exp = "";
        for (let i = 0; i < words.length; i++) {
            exp += "(?=.*" + words[i].toLowerCase() + ".*)";
        }
        exp += ".+";


        return of(this.artikliOriginal.filter(it => (it.naziv.toLowerCase().match(exp) != null)
            &&
            (
                kategorija == null ||
                (it.vrste && it.vrste.filter(vr => vr == kategorija).length > 0)
            )
            &&
            (
            dobavljac==null || dobavljac == it.dobavljac
            )
            &&
            (dostupnost == "0"
                ||
                (it.dostupnost != null && it.dostupnost != "0"))
        )
        );
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
            this.startSearch(text, null, this.selectedDostupnost,this.selectedDobavljac);
        });

        //this.selectedDobavljac = "--Svi--";
    }
    selectedDobavljac: string;

    private startSearch(text: string, kategorija: string, dostupnost: string,dobavljac:string) {
        this.selectedDostupnost = dostupnost;
        this.selectedKategorija = kategorija;
        this.selectedDobavljac = dobavljac;
        this.isSearching = true;
        if (kategorija == "--Sve--")
            kategorija = null;
        if (dobavljac == "--Svi--")
            dobavljac = null;


        this.searchArtikli(this.searchText, kategorija, this.selectedDostupnost, dobavljac).subscribe((res) => {
            this.isSearching = false;
            this.artikli = res;
            if (kategorija == null) {
                this.kategorije = new Array<string>();
                this.kategorije.push("--Sve--");

                this.artikli.forEach(a => {
                    if (a.vrste != null)
                        a.vrste.forEach(v => this.kategorije.push("[" + a.dobavljac + "] " + v));
                });
                let katgs = []
                katgs = this.kategorije.sort((a, b) => {
                    if (a < b)
                        return -1;
                    else if (a > b)
                        return 1;
                    else
                        return 0;
                })

                this.distinctKategorije = new Set(katgs);
            }
        }, (err) => {
            this.isSearching = false;
            console.log('error', err);
        });

    }

    sortProperty(property) {
        this.sortColumn = property;
        if (property == "sifra")
            this.sort(p => p.sifra, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "naziv")
            this.sort(p => p.naziv, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "cijena_sa_rabatom")
            this.sort(p => p.cijena_sa_rabatom, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "dostupnost")
            this.sort(p => p.dostupnost, this.sortOrder == true ? "ASC" : "DESC");

    }
    sort<T>(prop: (c: Artikal) => T, order: "ASC" | "DESC"): void {
        this.artikli.sort((a, b) => {
            if (prop(a) < prop(b))
                return -1;
            if (prop(a) > prop(b))
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

    constructor(private sanitizer: DomSanitizer, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private authenticationService: AuthenticationService, private router: Router) {
        http.get<Artikal[]>(baseUrl + 'webShopSync/artikli').subscribe(result => {
            //this.artikli = result;
            Object.assign(this.artikliOriginal, result);
        }, error => console.error(error));
        this.sortColumn = 'naziv';
        this.sortOrder = true;

        this.authenticationService.currentUser.subscribe(x => {
            this.currentUser = x;
            if (this.currentUser == null)
                this.router.navigate(['/login']);
        });

    }
}

