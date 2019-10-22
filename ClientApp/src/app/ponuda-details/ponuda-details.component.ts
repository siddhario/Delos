import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { jsonIgnore } from 'json-ignore';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-ponuda-details',
    templateUrl: './ponuda-details.component.html'
})


export class PonudaDetailsComponent implements OnInit {
    ngOnInit(): void {
        if (this.selectedPonuda != undefined) {
            this.http.get<PonudaStavka[]>(this.baseUrl + 'ponuda_stavka?ponuda_broj=' + this.selectedPonuda.broj).subscribe(result => {
                this.selectedPonuda.stavke = result;
            }, error => console.error(error));
        }
    }
    public stavka: PonudaStavka;
    public selectedPonuda: Ponuda;
    itemEdit: boolean = false;
    public itemAdd: boolean = false;
    startItemEdit(stavka: PonudaStavka) {
        this.stavka = stavka;
        this.stavka.editing = !this.stavka.editing;
        if (this.stavka.editing == false && stavka.stavka_broj == null) {
            this.selectedPonuda.stavke.splice(0, 1);
            this.itemAdd = false;
        }
    }

    savePonuda(ponuda: Ponuda) {
        if (ponuda.broj == undefined)
            this.http.post<Ponuda>(this.baseUrl + 'ponuda', ponuda).subscribe(result => {
                console.log("OK");
                //stavka.editing = false;

                //this.reloadItem(continueAdd);
                //this.itemAdd = false;
            }, error => console.error(error));
        else
            this.http.put<Ponuda>(this.baseUrl + 'ponuda', ponuda).subscribe(result => {
                console.log("OK");
                //this.reloadItem(false);
                //stavka.editing = false;
            }, error => console.error(error));
        //this.activeModal.close();
    }


    startItemAdd() {
        if (this.itemAdd == true)
            return;
        this.itemAdd = true;
        var newStavka = new PonudaStavka();
        newStavka.pdv_stopa = 17;
        newStavka.rabat_procenat = 0;
        newStavka.marza_procenat = 35;
        newStavka.jedinica_mjere = "KOM";
        newStavka.kolicina = 1;
        newStavka.ponuda_broj = this.selectedPonuda.broj;
        this.selectedPonuda.stavke = (new Array<PonudaStavka>(newStavka)).concat(this.selectedPonuda.stavke);
        this.startItemEdit(newStavka);
    }


    deleteStavka(stavka: PonudaStavka) {
        this.http.get(this.baseUrl + 'ponuda/stavka_delete?ponuda_broj=' + stavka.ponuda_broj + "&stavka_broj=" + stavka.stavka_broj).subscribe(result => {
            console.log("OK");
            var ponuda = this.selectedPonuda;
            let index = ponuda.stavke.findIndex(d => d.stavka_broj === stavka.stavka_broj);
            ponuda.stavke.splice(index, 1);//remove element from array
            this.reloadItem(false);
        }, error => console.error(error));
    }

    reloadItem(continueAdd: boolean) {
        this.http.get<Ponuda>(this.baseUrl + 'ponuda/getbybroj?broj=' + this.selectedPonuda.broj).subscribe(result => {
            this.selectedPonuda = result;
            if (continueAdd == true)
                this.startItemAdd();
        }, error => console.error(error));
    }
    cancel() {
        this.activeModal.close();
    }

    save(stavka: PonudaStavka, continueAdd: boolean) {
        stavka.cijena_nabavna = +stavka.cijena_nabavna;
        stavka.marza_procenat = +stavka.marza_procenat;
        stavka.kolicina = +stavka.kolicina;
        stavka.rabat_procenat = +stavka.rabat_procenat;
        stavka.cijena_sa_pdv = +stavka.cijena_sa_pdv;
        stavka.pdv_stopa = +stavka.pdv_stopa;
        stavka.cijena_bez_pdv = +stavka.cijena_bez_pdv;
        stavka.cijena_bez_pdv_sa_rabatom = +stavka.cijena_bez_pdv_sa_rabatom;
        if (stavka.stavka_broj == undefined)
            this.http.post<PonudaStavka>(this.baseUrl + 'ponuda/stavka_add', stavka).subscribe(result => {
                console.log("OK");
                stavka.editing = false;

                this.reloadItem(continueAdd);
                this.itemAdd = false;
            }, error => console.error(error));
        else
            this.http.put<PonudaStavka>(this.baseUrl + 'ponuda/stavka_update', stavka).subscribe(result => {
                console.log("OK");
                this.reloadItem(false);
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


    rowClass(ponuda: Ponuda) {
        if (ponuda.selected)
            return 'rowSelected';
        else
            return 'row';
    }

    constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public activeModal: NgbActiveModal) {
        this.selectedPonuda = new Ponuda();
        this.selectedPonuda.status = "E";
        this.selectedPonuda.radnik = "dario";
        this.selectedPonuda.datum = (new Date());
        
    }


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
    partner_jib: string;
    rok_vazenja: string;
    rok_isporuke: string;
    paritet: string;
    paritet_kod: string;
    valuta_placanja: string;
    pdv: number;
    iznos_sa_pdv: number;
    predmet: string;
    status: string;
    radnik: string;
    stavke: PonudaStavka[];
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
