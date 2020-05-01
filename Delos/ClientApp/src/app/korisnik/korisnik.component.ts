import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Korisnik } from '../model/korisnik';

@Component({
    selector: 'app-korisnik',
    templateUrl: './korisnik.component.html'
})
export class KorisnikComponent {
    public korisnici: Korisnik[];

    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        http.get<Korisnik[]>(baseUrl + 'korisnik').subscribe(result => {
            this.korisnici = result;
        }, error => console.error(error));
    }
}

