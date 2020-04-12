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

    @ViewChild('searchInput', { static: false }) searchInput: ElementRef<any>;


    artikliOriginal: Array<Artikal> = [];
    isSearching: boolean;
    searchArtikli(searchText: string): Observable<any> {
        if (!searchText || this.searchText.length < 3)
            return of(this.artikliOriginal);
        let words = this.searchText.split(" ");
        let exp = "";
        for (let i = 0; i < words.length; i++) {
            exp += "(?=.*" + words[i].toLowerCase() + ".*)";
        }
        exp += ".+";

        
        return of(this.artikliOriginal.filter(it => it.naziv.toLowerCase().match(exp) != null));
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
            , filter(res => res.length > 2 || res=="")
            // Time in milliseconds between key events
            , debounceTime(1000)
            // If previous query is diffent from current   
            , distinctUntilChanged()
            // subscription for response
        ).subscribe((text: string) => {
            this.isSearching = true;
            this.searchArtikli(text).subscribe((res) => {
                console.log('res', res);
                this.isSearching = false;
                this.artikli = res;
            }, (err) => {
                this.isSearching = false;
                console.log('error', err);
            });
        });
    }

    constructor(private sanitizer:DomSanitizer, http: HttpClient, @Inject('BASE_URL') baseUrl: string, private authenticationService: AuthenticationService, private router: Router) {
        http.get<Artikal[]>(baseUrl + 'webShopSync/artikli').subscribe(result => {
            this.artikli = result;
            Object.assign(this.artikliOriginal, this.artikli);
        }, error => console.error(error));

        this.authenticationService.currentUser.subscribe(x => {
            this.currentUser = x;
            if (this.currentUser == null)
                this.router.navigate(['/login']);
        });

    }
}

