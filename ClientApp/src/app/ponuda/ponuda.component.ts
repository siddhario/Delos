import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { jsonIgnore } from 'json-ignore';

@Component({
    selector: 'app-ponuda',
    templateUrl: './ponuda.component.html'
})


export class PonudaComponent {
    public ponude: Ponuda[];

    public stavka: PonudaStavka;
    itemEdit: boolean = false;
    startItemEdit(stavka: PonudaStavka) {
        this.stavka = stavka;
        this.stavka.editing = !this.stavka.editing;
    }


    deleteStavka(stavka: PonudaStavka) {
        this.http.get(this.baseUrl + 'ponuda/stavka_delete?ponuda_broj=' + stavka.ponuda_broj + "&stavka_broj=" + stavka.stavka_broj).subscribe(result => {
            console.log("OK");
            var ponuda = this.ponude.find(d => d.broj === stavka.ponuda_broj);
            let index = ponuda.stavke.findIndex(d => d.stavka_broj === stavka.stavka_broj);
            ponuda.stavke.splice(index, 1);//remove element from array
        }, error => console.error(error));
    }

    save(stavka: PonudaStavka) {
        stavka.cijena_nabavna = +stavka.cijena_nabavna;
        stavka.marza_procenat = +stavka.marza_procenat;
        stavka.kolicina = +stavka.kolicina;
        stavka.rabat_procenat = +stavka.rabat_procenat;
        stavka.cijena_sa_pdv = +stavka.cijena_sa_pdv;
        stavka.pdv_stopa = +stavka.pdv_stopa;
        stavka.cijena_bez_pdv = +stavka.cijena_bez_pdv;
        stavka.cijena_bez_pdv_sa_rabatom = +stavka.cijena_bez_pdv_sa_rabatom;
        this.http.post<Ponuda>(this.baseUrl + 'ponuda/stavka_update', stavka).subscribe(result => {
            console.log("OK");
            stavka.editing = false;
        }, error => console.error(error));
    }

    calculate() {
        this.stavka.vrijednost_nabavna = +(this.stavka.kolicina * this.stavka.cijena_nabavna).toFixed(2);
        this.stavka.ruc = +(this.stavka.vrijednost_nabavna * this.stavka.marza_procenat / 100).toFixed(2);
        this.stavka.iznos_bez_pdv = +(this.stavka.vrijednost_nabavna + this.stavka.ruc).toFixed(2);
        this.stavka.cijena_bez_pdv = +(this.stavka.iznos_bez_pdv / this.stavka.kolicina).toFixed(2);
        this.stavka.rabat_iznos = +(this.stavka.iznos_bez_pdv * this.stavka.rabat_procenat / 100).toFixed(2);
        this.stavka.cijena_bez_pdv_sa_rabatom = +((this.stavka.iznos_bez_pdv - this.stavka.rabat_iznos) / this.stavka.kolicina).toFixed(2);
        this.stavka.iznos_bez_pdv_sa_rabatom = +(this.stavka.iznos_bez_pdv - this.stavka.rabat_iznos).toFixed(2);
        this.stavka.iznos_sa_pdv = +(this.stavka.iznos_bez_pdv_sa_rabatom * (1 + this.stavka.pdv_stopa / 100)).toFixed(2);
        this.stavka.cijena_sa_pdv = +(this.stavka.iznos_sa_pdv / this.stavka.kolicina).toFixed(2);
        this.stavka.pdv = +(this.stavka.iznos_sa_pdv - this.stavka.iznos_bez_pdv_sa_rabatom).toFixed(2);
    }

    calculate2() {
        this.stavka.iznos_bez_pdv = +(this.stavka.kolicina * this.stavka.cijena_bez_pdv).toFixed(2);
        this.stavka.ruc = +(this.stavka.iznos_bez_pdv - this.stavka.vrijednost_nabavna).toFixed(2);
        this.stavka.marza_procenat = +(this.stavka.ruc / this.stavka.vrijednost_nabavna * 100).toFixed(2);
        this.stavka.rabat_iznos = +(this.stavka.iznos_bez_pdv * this.stavka.rabat_procenat / 100).toFixed(2);
        this.stavka.cijena_bez_pdv_sa_rabatom = +((this.stavka.iznos_bez_pdv - this.stavka.rabat_iznos) / this.stavka.kolicina).toFixed(2);
        this.stavka.iznos_bez_pdv_sa_rabatom = +(this.stavka.iznos_bez_pdv - this.stavka.rabat_iznos).toFixed(2);
        this.stavka.iznos_sa_pdv = +(this.stavka.iznos_bez_pdv_sa_rabatom * (1 + this.stavka.pdv_stopa / 100)).toFixed(2);
        this.stavka.cijena_sa_pdv = +(this.stavka.iznos_sa_pdv / this.stavka.kolicina).toFixed(2);
        this.stavka.pdv = +(this.stavka.iznos_sa_pdv - this.stavka.iznos_bez_pdv_sa_rabatom).toFixed(2);
    }

    calculate3() {
        this.stavka.rabat_iznos = +(this.stavka.iznos_bez_pdv * this.stavka.rabat_procenat / 100).toFixed(2);
        this.stavka.cijena_bez_pdv_sa_rabatom = +((this.stavka.iznos_bez_pdv - this.stavka.rabat_iznos) / this.stavka.kolicina).toFixed(2);
        this.stavka.iznos_bez_pdv_sa_rabatom = +(this.stavka.iznos_bez_pdv - this.stavka.rabat_iznos).toFixed(2);
        this.stavka.iznos_sa_pdv = +(this.stavka.iznos_bez_pdv_sa_rabatom * (1 + this.stavka.pdv_stopa / 100)).toFixed(2);
        this.stavka.cijena_sa_pdv = +(this.stavka.iznos_sa_pdv / this.stavka.kolicina).toFixed(2);
        this.stavka.pdv = +(this.stavka.iznos_sa_pdv - this.stavka.iznos_bez_pdv_sa_rabatom).toFixed(2);
    }

    calculate4() {
        this.stavka.iznos_sa_pdv = +(this.stavka.iznos_bez_pdv_sa_rabatom * (1 + this.stavka.pdv_stopa / 100)).toFixed(2);
        this.stavka.cijena_sa_pdv = +(this.stavka.iznos_sa_pdv / this.stavka.kolicina).toFixed(2);
        this.stavka.pdv = +(this.stavka.iznos_sa_pdv - this.stavka.iznos_bez_pdv_sa_rabatom).toFixed(2);
    }   
    
    selectItem(ponuda: Ponuda) {
        this.http.get<PonudaStavka[]>(this.baseUrl + 'ponuda_stavka?ponuda_broj=' + ponuda.broj).subscribe(result => {
            ponuda.stavke = result;
        }, error => console.error(error));
        this.ponude.filter(dd => dd.broj != ponuda.broj).forEach((value) => { value.selected = false });
        ponuda.selected = !ponuda.selected;
    }
    rowClass(ponuda: Ponuda) {
        if (ponuda.selected)
            return 'rowSelected';
        else
            return 'row';
    }

    constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string) {
        this.load();
    }

    load() {
        this.http.get<Ponuda[]>(this.baseUrl + 'ponuda').subscribe(result => {
            this.ponude = result;
        }, error => console.error(error));
    }

    searchText: string;
}

class Ponuda {
    broj: string;
    datum: Date;
    selected: boolean;
    partner_naziv: string;
    partner_adresa: string;
    partner_telefon: string;
    partner_email: string;
    iznos_sa_rabatom: number;
    pdv: number;
    iznos_sa_pdv: number;
    predmet: string;
    stavke: PonudaStavka[];
}
@Pipe({
    name: 'filter'
})
export class FilterPipe implements PipeTransform {
    transform(items: Ponuda[], searchText: string): any[] {
        if (!items) return [];
        if (!searchText) return items;
        searchText = searchText.toLowerCase();
        return items.filter(it => {
            return it.partner_naziv.toLowerCase().includes(searchText.toLowerCase())
                || (!!it.stavke &&
                    it.stavke.filter(s =>
                        (!!s.artikal_naziv && s.artikal_naziv.toLowerCase().includes(searchText))
                        ||
                        (!!s.opis && s.opis.toLowerCase().includes(searchText))
                    ).length > 0);
        });
    }
}


class PonudaStavka {
    ponuda_broj: string;
    stavka_broj: number;
    artikal_naziv: string;
    opis: string;
    jedinica_mjere: string;
    kolicina: number = null;  
    cijena_bez_pdv: number;
    cijena_bez_pdv_sa_rabatom: number;
    rabat_procenat: number;
    rabat_iznos: number;
    iznos_bez_pdv: number;
    iznos_bez_pdv_sa_rabatom: number;
    cijena_nabavna: number;
    vrijednost_nabavna: number;
    marza_procenat: number;
    ruc: number;
    pdv_stopa: number;
    pdv: number;

    cijena_sa_pdv: number;
    iznos_sa_pdv: number;

    @jsonIgnore()
    editing: boolean;
}
