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
    predmet: string;
}
