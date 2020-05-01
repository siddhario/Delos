import { Component, Inject, PipeTransform, Pipe } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerDetailsComponent } from '../partner-details/partner-details.component';
import { AuthenticationService } from '../auth/auth.service';
import { partner } from '../model/partner';
import { Korisnik } from '../model/korisnik';
import { Router } from '@angular/router';

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


    currentUser: Korisnik;
    constructor(private router: Router,private authenticationService: AuthenticationService, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private modalService: NgbModal) {
        this.load();
        this.authenticationService.currentUser.subscribe(x => {
            this.currentUser = x;
            if (this.currentUser == null)
                this.router.navigate(['/login']);
        });
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

