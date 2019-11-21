import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { partner } from '../model/partner';

@Component({
    selector: 'app-partner-details',
    templateUrl: './partner-details.component.html'
})


export class PartnerDetailsComponent{

    public selectedPartner: partner;
    itemEdit: boolean = false;
    public itemAdd: boolean = false;
   

    savepartner(partner: partner) {

        if (partner.sifra == undefined) {
            this.http.post<partner>(this.baseUrl + 'partner', partner).subscribe(result => {
                console.log("OK");
                this.toastr.success("Partner je uspješno dodat..");
                this.activeModal.close("OK");
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        } else {
            //partner.datum = new Date(partner.datum.getFullYear(), partner.datum.getMonth(), partner.datum.getDate());
            this.http.put<partner>(this.baseUrl + 'partner', partner).subscribe(result => {
                console.log("OK");
                this.toastr.success("Partner je uspješno sačuvan..");
                this.activeModal.close("OK");
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
            //this.activeModal.close();
        }
    }

    delete(sifra) {
       
        this.http.delete(this.baseUrl + 'partner?sifra='+sifra).subscribe(result => {
            console.log("OK");
            this.toastr.success("Partner je uspješno obrisan..");
            this.activeModal.close("DELETE");
        }, error => {
            this.toastr.error("Greška..");
            console.error(error)
        });
    }


    

    reloadItem(continueAdd: boolean) {
        this.http.get<partner>(this.baseUrl + 'ponuda/getbybroj?broj=' + this.selectedPartner.sifra).subscribe(result => {
            this.selectedPartner = result;
          
        }, error => {
            this.toastr.error("Greška..");
            console.error(error)
        });
    }
    cancel() {
        this.activeModal.close();
    }

   


    rowClass(ponuda: partner) {
        if (ponuda.selected)
            return 'rowSelected';
        else
            return 'row';
    }

    constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public activeModal: NgbActiveModal, private toastr: ToastrService) {
        this.startAdd();
    }

    startAdd() {
        this.selectedPartner = new partner();
        //this.selectedPartner.status = "E";
        //this.selectedPartner.radnik = "dario";
        //this.selectedPartner.datum = (new Date());
    }


}

