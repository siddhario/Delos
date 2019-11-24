import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { PonudaDetailsComponent } from '../ponuda-details/ponuda-details.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthenticationService } from '../auth/auth.service';
import { Ponuda } from '../model/ponuda';
import { Korisnik } from '../model/korisnik';
import { Router } from '@angular/router';
@Component({
    selector: 'app-ponuda',
    templateUrl: './ponuda.component.html'
})
export class PonudaComponent {
    public ponude: Ponuda[];
    public selectedPonuda: Ponuda;
    itemAdd: boolean = false;
    sortOrder: boolean;
    sortColumn: string;
    searchText: string;
    currentUser: Korisnik;
    constructor(private router: Router, private authenticationService: AuthenticationService, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private modalService: NgbModal) {
        this.load();
        this.sortColumn = 'broj';
        this.sortOrder = true;

        this.authenticationService.currentUser.subscribe(x => {
            this.currentUser = x;
            if (this.currentUser == null)
                this.router.navigate(['/login']);
        });
    }
    sortProperty(property) {
        this.sortColumn = property;
        if (property == "broj")
            this.sort(p => p.broj, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "datum")
            this.sort(p => p.datum, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "partner_naziv")
            this.sort(p => p.partner_naziv, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "partner_adresa")
            this.sort(p => p.partner_adresa, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "partner_telefon")
            this.sort(p => p.partner_telefon, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "iznos_sa_rabatom")
            this.sort(p => p.iznos_sa_rabatom, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "pdv")
            this.sort(p => p.pdv, this.sortOrder == true ? "ASC" : "DESC");
        else if (property == "iznos_sa_pdv")
            this.sort(p => p.iznos_sa_pdv, this.sortOrder == true ? "ASC" : "DESC");
    }
    sort<T>(prop: (c: Ponuda) => T, order: "ASC" | "DESC"): void {
        this.ponude.sort((a, b) => {
            if (prop(a) < prop(b))
                return -1;
            if (prop(a) > prop(b))
                return 1;
            return 0;
        });

        if (order === "DESC") {
            this.ponude.reverse();
            this.sortOrder = true;
        } else {
            this.sortOrder = false;
        }
    }
    add() {
        let modalRef = this.modalService.open(PonudaDetailsComponent
            , {
                size: "xl",
                windowClass: 'modal-xl',
                backdrop: 'static'
            }
        );
        modalRef.componentInstance.startAdd();
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
                windowClass: 'modal-xl',
                backdrop: 'static'
            });
        modalRef.componentInstance.selectedPonuda = ponuda;
        modalRef.result.then((data) => {

            this.load();

        }, (reason) => {
            this.load();
        });
    }
    load() {
        this.http.get<Ponuda[]>(this.baseUrl + 'ponuda').subscribe(result => {
            this.ponude = result;
        }, error => console.error(error));
    }
}


