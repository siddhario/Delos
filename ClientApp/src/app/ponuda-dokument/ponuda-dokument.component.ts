import { Component, Inject, PipeTransform, Pipe, Input, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerDetailsComponent } from '../partner-details/partner-details.component';
import { AuthenticationService } from '../auth/auth.service';
import { partner } from '../model/partner';
import { Korisnik } from '../model/korisnik';
import { Router } from '@angular/router';
import { PonudaDokument } from '../model/ponudaDokument';
import { ToastrService } from 'ngx-toastr';
import { Ponuda } from '../model/ponuda';
import { NgbdModalConfirm } from '../modal-focus/modal-focus.component';
import { PonudaStavka } from '../model/ponudaStavka';

@Component({
    selector: 'app-ponuda-dokument',
    templateUrl: './ponuda-dokument.component.html'
})
export class PonudaDokumentComponent implements OnInit {
    ngOnInit(): void {
        this.load();
        this.dokumentiVisible = true;
    }
    dokumentiVisible: boolean;
    public dokumenti: PonudaDokument[];
    @Input() ponudaStavka: PonudaStavka;
    @Input() ponuda: Ponuda;
    @Input() global: boolean;

    itemEdit: boolean = false;
    itemAdd: boolean = false;
    isExpanded = false;
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

    collapse() {
        this.isExpanded = false;
    }
    toggle() {
        this.isExpanded = !this.isExpanded;
    }

    currentUser: Korisnik;
    constructor(private toastr: ToastrService, private router: Router, private authenticationService: AuthenticationService, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private modalService: NgbModal) {

        this.authenticationService.currentUser.subscribe(x => {
            this.currentUser = x;
            if (this.currentUser == null)
                this.router.navigate(['/login']);
        });
    }
    fileToUpload: File = null;
    handleFileInput(files: FileList) {
        this.fileToUpload = files.item(0);

        var oReq = new XMLHttpRequest();
        oReq.open("POST", this.baseUrl + 'ponuda/upload_dokument?ponudabroj=' + this.ponuda.broj + (this.ponudaStavka == null || this.global==true ? '' : '&stavkabroj=' + this.ponudaStavka.stavka_broj), true);
        oReq.onload = function (oEvent) {
            // Uploaded.
        };
        var form = new FormData();
        form.append("blob", this.fileToUpload, this.fileToUpload.name);
        oReq.setRequestHeader("Authorization", "Bearer " + this.currentUser.token);
        oReq.send(form);

        oReq.onreadystatechange = () => {
            if (oReq.readyState == 4 && oReq.status == 200) {
                this.toastr.success("Dokument je uspješno povezan...");

                this.load();
            }
            else if (oReq.readyState == 4 && oReq.status != 200) {
                this.toastr.error("Greška...");
            }
        };
    }

    dokument_download(dokument) {
        this.http.get(this.baseUrl + 'ponuda/dokument_download?naziv=' + dokument.naziv + '&broj=' + dokument.ponuda_broj
            , {
                responseType: 'arraybuffer'
            }
        ).subscribe(response => this.downLoadFile(response, dokument.opis));
        return false;
    }

    downLoadFile(data: any, type: string) {
        let blob = new Blob([data], { type: type });
        let url = window.URL.createObjectURL(blob);
        let pwa = window.open(url);
        if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
            alert('Please disable your Pop-up blocker and try again.');
        }
    }

    deleteDokument(dokument: PonudaDokument) {
        let modalRef = this.modalService.open(NgbdModalConfirm);
        modalRef.result.then((data) => {
            this.http.get(this.baseUrl + 'ponuda/dokument_delete?ponuda_broj=' + dokument.ponuda_broj + "&dokument_broj=" + dokument.dokument_broj).subscribe(result => {
                console.log("OK");
                this.toastr.success("Dokument je uspješno obrisan..");
                this.load();
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        }, (reason) => {
        });

        modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati dokument " + dokument.naziv + " ?";

    }

    load() {
        this.http.get<PonudaDokument[]>(this.baseUrl + 'ponuda_dokument?ponuda_broj=' + this.ponuda.broj + (this.ponudaStavka == null ? '' : '&stavka_broj=' + this.ponudaStavka.stavka_broj)).subscribe(result => {
            this.dokumenti = result;
        }, error => console.error(error));
    }
}

