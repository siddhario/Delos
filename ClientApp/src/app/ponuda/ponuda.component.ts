import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-ponuda',
    templateUrl: './ponuda.component.html'
})
export class PonudaComponent {
    public ponude: Ponuda[];

    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        http.get<Ponuda[]>(baseUrl + 'ponuda').subscribe(result => {
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
