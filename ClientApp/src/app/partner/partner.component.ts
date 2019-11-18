import { Component, Inject, PipeTransform, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerDetailsComponent } from '../partner-details/partner-details.component';

@Component({
    selector: 'app-partner',
    templateUrl: './partner.component.html'
})
export class PartnerComponent {
    public partneri: partner[];


    searchText: string;

    public selectedPartner: partner;

    itemEdit: boolean = false;
    itemAdd: boolean = false;

    add() {
        let modalRef = this.modalService.open(PartnerDetailsComponent
            , {
                size: 'lg',
                windowClass: 'modal-xl',
                backdrop: 'static'
            }
        );
        modalRef.componentInstance.itemAdd = true;
        modalRef.result.then((data) => {

            this.load();

        }, (reason) => {
            this.load();
        });
    }

    selectItem(partner: partner) {

        this.partneri.filter(dd => dd.sifra != partner.sifra).forEach((value) => { value.selected = false });
        partner.selected = !partner.selected;
        if (partner.selected == true)
            this.selectedPartner = partner;

        let modalRef = this.modalService.open(PartnerDetailsComponent
            , {
                size: "xl",
                windowClass: 'modal-xl'
            });
        modalRef.componentInstance.selectedPartner = partner;
        modalRef.result.then((data) => {
         
                this.load();
         
        }, (reason) => {
                this.load();
        });
    }
    rowClass(partner: partner) {
        if (partner.selected)
            return 'rowSelected';
        else
            return 'row';
    }


    constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private modalService: NgbModal) {
        this.load();
    }

    load() {
        this.http.get<partner[]>(this.baseUrl + 'partner').subscribe(result => {
            this.partneri = result;
        }, error => console.error(error));
    }
}
@Pipe({
    name: 'filterPartner'
})
export class FilterPartnerPipe implements PipeTransform {
    transform(items: partner[], searchText: string): any[] {
        if (!items) return [];
        if (!searchText) return items;
        searchText = searchText.toLowerCase();
        return items.filter(it => {
            return it.naziv.toLowerCase().includes(searchText.toLowerCase());
        });
    }
}


export class partner {
    sifra: number;
    naziv: string;
    tip: string;
    email: string;
    adresa: string;
    telefon: string;
    kupac: boolean;
    dobavljac: boolean;
    broj_lk: string;
    selected?: boolean;
    maticni_broj: string;
}
