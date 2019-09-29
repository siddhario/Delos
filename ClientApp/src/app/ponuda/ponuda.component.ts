import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-ponuda',
    templateUrl: './ponuda.component.html'
})


export class PonudaComponent {
    public ponude: Ponuda[];

    constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string) {
        this.load();
    }

    load() {
        this.http.get<Ponuda[]>(this.baseUrl + 'ponuda').subscribe(result => {
            this.ponude = result;
        }, error => console.error(error));
    }
}

interface Ponuda {
    broj: string;
    datum: Date;
    partner_naziv: string;
    partner_adresa: string;
    partner_telefon: string;
    partner_email: string;
    iznos_sa_rabatom: number;
    pdv: number;
    iznos_sa_pdv: number;
    predmet: string;
}
