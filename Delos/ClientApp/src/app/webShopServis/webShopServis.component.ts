import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Korisnik } from '../model/korisnik';
import { WebShopServis } from '../model/webShopServis';

@Component({
    selector: 'app-web-shop-servis',
    templateUrl: './webShopServis.component.html'
})
export class WebShopServisComponent {
    public servisi: WebShopServis[];

    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        http.get<WebShopServis[]>(baseUrl + 'webShopSync/list').subscribe(result => {
            this.servisi = result;
        }, error => console.error(error));
    }
}

