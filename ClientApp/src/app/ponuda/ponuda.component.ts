import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { jsonIgnore } from 'json-ignore';
import { PonudaDetailsComponent } from '../ponuda-details/ponuda-details.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
@Component({
    selector: 'app-ponuda',
    templateUrl: './ponuda.component.html'
})


export class PonudaComponent {
    public ponude: Ponuda[];
    public selectedPonuda: Ponuda;

    itemEdit: boolean = false;
    itemAdd: boolean = false;

    add() {
        let modalRef = this.modalService.open(PonudaDetailsComponent
            , {
                size: "xl",
                windowClass: 'modal-xl'
            }
        );
        modalRef.componentInstance.itemAdd = true;
        modalRef.result.then((data) => {

            this.load();

        }, (reason) => {
            this.load();
        });
    }

    selectItem(ponuda: Ponuda) {

        this.ponude.filter(dd => dd.broj != ponuda.broj).forEach((value) => { value.selected = false });
        ponuda.selected = !ponuda.selected;
        if (ponuda.selected == true)
            this.selectedPonuda = ponuda;

        let modalRef = this.modalService.open(PonudaDetailsComponent
            , {
                size: "xl",
                windowClass: 'modal-xl'
            });
        modalRef.componentInstance.selectedPonuda = ponuda;
        modalRef.result.then((data) => {

            this.load();

        }, (reason) => {
            this.load();
        });
    }
    rowClass(ponuda: Ponuda) {
        if (ponuda.selected)
            return 'rowSelected';
        else
            return 'row';
    }

    constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private modalService: NgbModal) {
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
    @jsonIgnore()
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
    @jsonIgnore()
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
