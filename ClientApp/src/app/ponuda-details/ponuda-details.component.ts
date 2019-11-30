import { Component, Inject, OnInit, ViewChild, ElementRef, ViewChildren, QueryList } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Pipe, PipeTransform } from '@angular/core';
import { jsonIgnore } from 'json-ignore';
import { NgbActiveModal, NgbTypeahead, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, catchError, map } from 'rxjs/operators';
import { NgbdModalConfirm } from '../modal-focus/modal-focus.component';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import { environment } from '../../environments/environment';
import { AuthenticationService } from '../auth/auth.service';
import { Ponuda } from '../model/ponuda';
import { PonudaStavka } from '../model/ponudaStavka';
import { partner } from '../model/partner';
import { Korisnik } from '../model/korisnik';
import { FormMode } from '../enums/formMode';
import { PonudaDokument } from '../model/ponudaDokument';
import { FormGroup, FormBuilder } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'app-ponuda-details',
    templateUrl: './ponuda-details.component.html'
})


export class PonudaDetailsComponent implements OnInit {
    public model: any;
    public formMode: FormMode;
    currentUser: Korisnik;
    stavkeVisible: boolean = true;
    ponudaVisible: boolean = true;
    dokumentiVisible: boolean = true;
    isExpanded = false;
    public selectedPonuda: Ponuda;
    dokumentForm: FormGroup;

    @ViewChild("partnerElement", { static: false }) partnerElement: ElementRef;
    @ViewChildren("naziv") naziv: QueryList<ElementRef>;

    constructor(private _sanitizer: DomSanitizer,
        private formBuilder: FormBuilder, private authenticationService: AuthenticationService, private modalService: NgbModal, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public activeModal: NgbActiveModal, private toastr: ToastrService) {
        toastr.toastrConfig.positionClass = "toast-bottom-right";
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
        this.formMode = FormMode.View;
        this.dokumentForm = this.formBuilder.group({
            dokument: ''
        });
    }

    ngOnInit(): void {
        if (this.selectedPonuda != undefined) {
            this.http.get<PonudaStavka[]>(this.baseUrl + 'ponuda_stavka?ponuda_broj=' + this.selectedPonuda.broj).subscribe(result => {
                result.forEach(s => s.mode = FormMode.View);
                this.selectedPonuda.stavke = result;
                if (this.selectedPonuda.stavke != null)
                    this.selectedPonuda.stavke.forEach(s => s.mode = FormMode.View);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });

            this.http.get<PonudaDokument[]>(this.baseUrl + 'ponuda_dokument?ponuda_broj=' + this.selectedPonuda.broj).subscribe(result => {
                result.forEach(s => s.mode = FormMode.View);
                this.selectedPonuda.dokumenti = result;
                if (this.selectedPonuda.dokumenti != null)
                    this.selectedPonuda.dokumenti.forEach(s => s.mode = FormMode.View);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        }
    }
    public startAdd() {
        this.formMode = FormMode.Add;
        this.ponudaVisible = true;
        this.selectedPonuda = new Ponuda();
        this.selectedPonuda.status = "E";
        this.selectedPonuda.datum = (new Date());
        this.selectedPonuda.rok_isporuke = "3-5 dana";
        this.selectedPonuda.rok_vazenja = "7 dana";
        this.selectedPonuda.valuta_placanja = "Avans";
        setTimeout(_ => {
            this.partnerElement.nativeElement.focus();
        }, 0);
    }
    savePonuda(ponuda) {
        if ((typeof ponuda.partner) == "string") {
            ponuda.partner_naziv = ponuda.partner;
            ponuda.partner = new partner();
            ponuda.partner.naziv = ponuda.partner_naziv;
        }
        if (ponuda.broj == undefined) {
            let obj: object = ponuda.datum;
            this.http.post<Ponuda>(this.baseUrl + 'ponuda', ponuda).subscribe(result => {
                console.log("OK");
                this.toastr.success("Ponuda je uspješno dodata..");
                this.formMode = FormMode.View;
                ponuda = result;
                this.selectedPonuda = ponuda;
                if (this.selectedPonuda.stavke != null)
                    this.selectedPonuda.stavke.forEach(s => s.mode = FormMode.View);
                if (this.selectedPonuda.dokumenti != null)
                    this.selectedPonuda.dokumenti.forEach(s => s.mode = FormMode.View);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        } else {
            this.http.put<Ponuda>(this.baseUrl + 'ponuda', ponuda).subscribe(result => {
                console.log("OK");
                this.toastr.success("Ponuda je uspješno sačuvana..");
                this.formMode = FormMode.View;
                ponuda = result;
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        }
    }
    izmijeniPonudu() {
        this.formMode = FormMode.Edit;
        setTimeout(_ => {
            this.partnerElement.nativeElement.focus();
        }, 0);
    }
    getStatus(status) {
        if (status == "E")
            return "Evidentirana";
        else if (status == "Z")
            return "Zaključena";
        else if (status == "R")
            return "Realizovana";
        else if (status == "D")
            return "Djelimično realizovana";
        else if (status == "N")
            return "Nerealizovana";
    }
    collapse() {
        this.isExpanded = false;
    }
    toggle() {
        this.isExpanded = !this.isExpanded;
    }
    sortStavke(stavke) {
        return stavke.sort((a, b) => {
            if (a.stavka_broj < b.stavka_broj)
                return -1;
            else if (a.stavka_broj > b.stavka_broj)
                return 1;
            else
                return 0;
        })
    }
    makeDocument(images) {
        var dd = {
            styles: {
                header: {
                    fontSize: 18,
                    bold: true,
                    margin: [0, 0, 0, 10],
                    alignment: 'center'
                },
                subheader: {
                    fontSize: 16,
                    bold: true,
                    margin: [0, 10, 0, 5]
                },
                tableExample: {
                    margin: [0, 5, 0, 15]
                },
                tableHeader: {
                    bold: true,
                    fontSize: 11,
                    color: 'black'
                }
            },
            footer: {},
            content: []
        };



        dd.content.push({ width: 510, image: environment.memo });
        dd.content.push({ text: '\n' });
        dd.content.push({
            text: 'PONUDA BROJ ' + this.selectedPonuda.broj,
            style: 'header',

        });
        dd.content.push({ text: '\n' });

        dd.content.push({
            alignment: 'justify',
            columns: [
                [
                    { columns: [{ text: 'Datum: ', bold: true, width: 60, alignment: 'left' }, { text: (new Date(this.selectedPonuda.datum)).toLocaleDateString('sr-Latn-ba'), width: 250, alignment: 'left' }] },
                    { columns: [{ text: 'Partner: ', bold: true, width: 60, alignment: 'left' }, { text: this.selectedPonuda.partner_naziv, width: 250, alignment: 'left' }] },
                    { columns: [{ text: 'JIB: ', bold: true, width: 60, alignment: 'left' }, { text: this.selectedPonuda.partner_jib, width: 250, alignment: 'left' }] },
                    { columns: [{ text: 'Adresa: ', bold: true, width: 60, alignment: 'left' }, { text: this.selectedPonuda.partner_adresa, width: 250, alignment: 'left' }] }

                ],
                [
                    { columns: [{ text: 'Valuta: ', bold: true, width: 95, alignment: 'left' }, { text: this.selectedPonuda.valuta_placanja, width: 105, alignment: 'left' }] },
                    { columns: [{ text: 'Opcija ponude: ', bold: true, width: 95, alignment: 'left' }, { text: this.selectedPonuda.rok_vazenja, width: 105, alignment: 'left' }] },
                    { columns: [{ text: 'Rok isporuke: ', bold: true, width: 95, alignment: 'left' }, { text: this.selectedPonuda.rok_isporuke, width: 105, alignment: 'left' }] },
                    { columns: [{ text: this.selectedPonuda.paritet != null ? 'Paritet: ' : '', bold: true, width: 95, alignment: 'left' }, { text: this.selectedPonuda.paritet, width: 105, alignment: 'left' }] }
                ]
            ]
        });

        dd.content.push('\n');

        var itemsTable = {
            style: 'tableExample', table: {
                headerRows: 1,
                widths: [520], body: [
                ]
            },
            layout: {
                fillColor: function (rowIndex, node, columnIndex) {
                    return (rowIndex === 0) ? 'lightgray' : null;
                },
                hLineWidth: function (i, node) {
                    if (i === 0 || i === 1 || i === node.table.body.length) {
                        return 2;
                    }

                    return 1;
                },
                hLineColor: function (i, node) {
                    if (i === 0 || i === 1 || i === node.table.body.length) {
                        return 'black';
                    }
                    return 'gray';
                },
                vLineWidth: function (i, node) {
                    return 0;
                },
            }
        };
        itemsTable.table.body.push([{
            columns: [
                { width: 25, text: "R.B.", style: 'tableHeader', alignment: 'center', margin: [0, 0, 5, 0], },
                {
                    alignment: 'left',
                    width: 245,
                    text: [
                        { text: 'Naziv artikla' + '\n', bold: true },
                        { text: 'Opis', fontSize: 10 }]
                },
                { width: 50, text: 'Količina', style: 'tableHeader', alignment: 'center' },
                { width: 40, text: 'JM', style: 'tableHeader', alignment: 'center' },
                { width: 60, text: 'Cijena bez PDV-a', style: 'tableHeader', alignment: 'center' },
                { width: 40, text: 'Rabat', style: 'tableHeader', alignment: 'center' },
                { width: 60, text: 'Iznos bez PDV-a', style: 'tableHeader', alignment: 'center' }]
        }]);

        this.selectedPonuda.stavke.sort((a, b) => {
            if (a.stavka_broj < b.stavka_broj)
                return -1;
            else if (a.stavka_broj > b.stavka_broj)
                return 1;
            else
                return 0;
        }).forEach((s, index) => {

            itemsTable.table.body.push([{
                unbreakable: true, columns: [
                    { width: 25, text: (index + 1) + ".", margin: [0, 0, 5, 0], alignment: 'center' },
                    {
                        width: 245,
                        text: [
                            { text: s.artikal_naziv + '\n', bold: true },
                            { text: s.opis, fontSize: 10 }]
                    },
                    { width: 50, text: s.kolicina, alignment: 'center' },
                    { width: 40, text: s.jedinica_mjere, alignment: 'center' },
                    { width: 60, text: s.cijena_bez_pdv.toFixed(2), alignment: 'right' },
                    { width: 40, text: s.rabat_iznos.toFixed(2), alignment: 'right' },
                    { width: 60, text: s.iznos_bez_pdv.toFixed(2), alignment: 'right' }]
            }],
            );
        });

        dd.content.push(itemsTable);

        dd.content.push({
            alignment: 'right',
            unbreakable: true,
            stack: [
                { columns: [{ text: "Ukupan iznos bez rabata:", width: 420, alignment: 'right', bold: true }, { text: this.selectedPonuda.iznos_bez_rabata.toFixed(2), width: 100, alignment: 'right' }] },
                { columns: [{ text: "Iznos rabata:", width: 420, alignment: 'right', bold: true }, { text: this.selectedPonuda.rabat.toFixed(2), width: 100, alignment: 'right' }] },
                { columns: [{ text: "Ukupan iznos bez PDV-a:", width: 420, alignment: 'right', bold: true }, { text: this.selectedPonuda.iznos_sa_rabatom.toFixed(2), width: 100, alignment: 'right' }] },
                { columns: [{ text: "PDV:", width: 420, alignment: 'right', bold: true }, { text: this.selectedPonuda.pdv.toFixed(2), width: 100, alignment: 'right' }] },
                { columns: [{ text: "Ukupan iznos sa PDV-om:", width: 420, alignment: 'right', bold: true }, { text: this.selectedPonuda.iznos_sa_pdv.toFixed(2), width: 100, alignment: 'right' }] }
            ]
        }
        );
        dd.content.push({ text: '\n' });
        dd.content.push({
            text: "Dokument sastavio: ", alignment: 'right', bold: true
        });
        dd.content.push({
            text: this.selectedPonuda.korisnik.ime + " " + this.selectedPonuda.korisnik.prezime, alignment: 'right'
        });
        dd.footer = { width: 510, image: environment.footer, alignment: 'center' };
        if (images != null)
            images.forEach(s => dd.content.push({ image: s, width: 520 }));
        return dd;
    }

    _arrayBufferToBase64(arrayBuffer) {
        //var binary = '';
        //var bytes = new Uint8Array(buffer);
        //var len = bytes.byteLength;
        //for (var i = 0; i < len; i++) {
        //    binary += String.fromCharCode(bytes[i]);
        //}
        //return window.btoa(binary);function base64ArrayBuffer(arrayBuffer) {
        var base64 = ''
        var encodings = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'

        var bytes = new Uint8Array(arrayBuffer)
        var byteLength = bytes.byteLength
        var byteRemainder = byteLength % 3
        var mainLength = byteLength - byteRemainder

        var a, b, c, d
        var chunk

        // Main loop deals with bytes in chunks of 3
        for (var i = 0; i < mainLength; i = i + 3) {
            // Combine the three bytes into a single integer
            chunk = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2]

            // Use bitmasks to extract 6-bit segments from the triplet
            a = (chunk & 16515072) >> 18 // 16515072 = (2^6 - 1) << 18
            b = (chunk & 258048) >> 12 // 258048   = (2^6 - 1) << 12
            c = (chunk & 4032) >> 6 // 4032     = (2^6 - 1) << 6
            d = chunk & 63               // 63       = 2^6 - 1

            // Convert the raw binary segments to the appropriate ASCII encoding
            base64 += encodings[a] + encodings[b] + encodings[c] + encodings[d]
        }

        // Deal with the remaining bytes and padding
        if (byteRemainder == 1) {
            chunk = bytes[mainLength]

            a = (chunk & 252) >> 2 // 252 = (2^6 - 1) << 2

            // Set the 4 least significant bits to zero
            b = (chunk & 3) << 4 // 3   = 2^2 - 1

            base64 += encodings[a] + encodings[b] + '=='
        } else if (byteRemainder == 2) {
            chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1]

            a = (chunk & 64512) >> 10 // 64512 = (2^6 - 1) << 10
            b = (chunk & 1008) >> 4 // 1008  = (2^6 - 1) << 4

            // Set the 2 least significant bits to zero
            c = (chunk & 15) << 2 // 15    = 2^4 - 1

            base64 += encodings[a] + encodings[b] + encodings[c] + '='
        }

        return base64;
    }

    //async imageblob(dokument) {

    //  let result =await this.http.get(this.baseUrl + 'ponuda/dokument_download?naziv=' + dokument.naziv + '&broj=' + dokument.ponuda_broj
    //        , {
    //            responseType: 'arraybuffer'
    //        }
    //    ).toPromise();
    //    return this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpg;base64,'
    //        + this._arrayBufferToBase64(result));
         
    //}
    async pdf() {
        let images = [];
        for (let i = 0; i < this.selectedPonuda.dokumenti.length; i++) {
            let asyncResult = await this.http.get(this.baseUrl + 'ponuda/dokument_download?naziv=' + this.selectedPonuda.dokumenti[i].naziv + '&broj=' + this.selectedPonuda.dokumenti[i].ponuda_broj
                , {
                    responseType: 'arraybuffer'
                }
            ).toPromise();
            images.push('data:' + this.selectedPonuda.dokumenti[i].opis + ';base64,' + this._arrayBufferToBase64(asyncResult));
        }

        var dd = this.makeDocument(images);
        pdfMake.vfs = pdfFonts.pdfMake.vfs;
        var pdf = pdfMake.createPdf(dd);
        pdf.download();
    }
    async email() {
        let images = [];
        for (let i = 0; i < this.selectedPonuda.dokumenti.length; i++) {
            let asyncResult = await this.http.get(this.baseUrl + 'ponuda/dokument_download?naziv=' + this.selectedPonuda.dokumenti[i].naziv + '&broj=' + this.selectedPonuda.dokumenti[i].ponuda_broj
                , {
                    responseType: 'arraybuffer'
                }
            ).toPromise();
            images.push('data:' + this.selectedPonuda.dokumenti[i].opis + ';base64,' + this._arrayBufferToBase64(asyncResult));
        }
        var dd = this.makeDocument(images);
        pdfMake.vfs = pdfFonts.pdfMake.vfs;
        var pdf = pdfMake.createPdf(dd);
        pdf.getBlob((b) => {
            var oReq = new XMLHttpRequest();
            oReq.open("POST", this.baseUrl + 'ponuda/uploadPDF?broj=' + this.selectedPonuda.broj, true);
            oReq.onload = function (oEvent) {
                // Uploaded.
            };
            var form = new FormData();
            form.append("blob", b, this.selectedPonuda.broj.replace('/', '_') + ".pdf");
            oReq.setRequestHeader("Authorization", "Bearer " + this.currentUser.token);
            oReq.send(form);

            oReq.onreadystatechange = () => {
                if (oReq.readyState == 4 && oReq.status == 200) {
                    let blob = new Blob([oReq.response], { type: 'message/rfc822' });
                    let url = window.URL.createObjectURL(blob);
                    let pwa = window.open(url);
                    if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
                        alert('Please disable your Pop-up blocker and try again.');
                    }
                }
                else if (oReq.readyState == 4 && oReq.status != 200) {
                    this.toastr.error("Greška! Provjerite e-mail adresu kupca i e-mail adresu vašeg korisničkog naloga!");
                }
            };
        });
    }
    zakljuciPonudu() {
        if (this.selectedPonuda != undefined) {
            let modalRef = this.modalService.open(NgbdModalConfirm);
            modalRef.result.then((data) => {
                this.http.put<Ponuda>(this.baseUrl + 'ponuda/zakljuciPonudu?broj=' + this.selectedPonuda.broj, null).subscribe(result => {
                    this.toastr.success("Ponuda je uspješno zaključena..");
                    this.selectedPonuda = result;
                    if (this.selectedPonuda.stavke != null)
                        this.selectedPonuda.stavke.forEach(s => s.mode = FormMode.View);
                    if (this.selectedPonuda.dokumenti != null)
                        this.selectedPonuda.dokumenti.forEach(s => s.mode = FormMode.View);
                }, error => {
                    if (error.status == 403)
                        this.toastr.error("Iznos ponude je veći od dozvoljenog!");
                    else
                        this.toastr.error("Greška..");
                    console.error(error)
                });
            }, (reason) => {
            });

            modalRef.componentInstance.confirmText = "Da li ste sigurni da želite zaključiti ponudu " + this.selectedPonuda.broj + " ?";
        }
    }
    downLoadFile(data: any, type: string) {
        let blob = new Blob([data], { type: type });
        let url = window.URL.createObjectURL(blob);
        let pwa = window.open(url);
        if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
            alert('Please disable your Pop-up blocker and try again.');
        }
    }
    excel() {
        if (this.selectedPonuda != undefined) {

            this.http.get(this.baseUrl + 'ponuda/excel?broj=' + this.selectedPonuda.broj
                , {
                    responseType: 'arraybuffer'
                }
            ).subscribe(response => this.downLoadFile(response, "application/vnd.ms-excel.sheet.macroEnabled.12"));
        }
    }
    statusiraj(status) {
        if (this.selectedPonuda != undefined) {
            let modalRef = this.modalService.open(NgbdModalConfirm);
            modalRef.result.then((data) => {
                this.http.put<Ponuda>(this.baseUrl + 'ponuda/statusiraj?broj=' + this.selectedPonuda.broj + "&status=" + status, null).subscribe(result => {
                    this.toastr.success("Status ponude je uspješno postavljen..");
                    this.selectedPonuda = result;
                    if (this.selectedPonuda.stavke != null)
                        this.selectedPonuda.stavke.forEach(s => s.mode = FormMode.View);
                    if (this.selectedPonuda.dokumenti != null)
                        this.selectedPonuda.dokumenti.forEach(s => s.mode = FormMode.View);
                }, error => {
                    this.toastr.error("Greška..");
                    console.error(error)
                });
            }, (reason) => {
            });

            modalRef.componentInstance.confirmText = "Da li ste sigurni da želite promijeniti status ponude "
                + this.selectedPonuda.broj + " u " + (status == 'R' ? '\'realizovana\'' : (status == 'D' ? '\'djelimično realizovana\'' : '\'nerealizovana\'')) + " ? ";
        }
    }
    otkljucajPonudu() {
        if (this.selectedPonuda != undefined) {
            let modalRef = this.modalService.open(NgbdModalConfirm);
            modalRef.result.then((data) => {
                this.http.put<Ponuda>(this.baseUrl + 'ponuda/otkljucajPonudu?broj=' + this.selectedPonuda.broj, null).subscribe(result => {
                    this.toastr.success("Ponuda je uspješno otključana..");
                    this.selectedPonuda = result;
                    if (this.selectedPonuda.stavke != null)
                        this.selectedPonuda.stavke.forEach(s => s.mode = FormMode.View);
                    if (this.selectedPonuda.dokumenti != null)
                        this.selectedPonuda.dokumenti.forEach(s => s.mode = FormMode.View);
                }, error => {
                    this.toastr.error("Greška..");
                    console.error(error)
                });
            }, (reason) => {
            });

            modalRef.componentInstance.confirmText = "Da li ste sigurni da želite otključati ponudu " + this.selectedPonuda.broj + " ?";
        }
    }
    obrisiPonudu() {
        if (this.selectedPonuda != undefined) {
            let modalRef = this.modalService.open(NgbdModalConfirm);
            modalRef.result.then((data) => {
                this.http.delete(this.baseUrl + 'ponuda/obrisiPonudu?broj=' + this.selectedPonuda.broj).subscribe(result => {
                    this.toastr.success("Ponuda je uspješno obrisana..");
                    this.activeModal.close();
                }, error => {
                    this.toastr.error("Greška..");
                    console.error(error)
                });
            }, (reason) => {
            });

            modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati ponudu " + this.selectedPonuda.broj + " ?";
        }
    }
    kopirajPonudu() {
        if (this.selectedPonuda != undefined) {
            let modalRef = this.modalService.open(NgbdModalConfirm);
            modalRef.result.then((data) => {
                this.http.put<Ponuda>(this.baseUrl + 'ponuda/kopirajPonudu?broj=' + this.selectedPonuda.broj, null).subscribe(result => {
                    this.toastr.success("Ponuda je uspješno kopirana..");
                    this.selectedPonuda = result;
                    if (this.selectedPonuda.stavke != null)
                        this.selectedPonuda.stavke.forEach(s => s.mode = FormMode.View);
                    if (this.selectedPonuda.dokumenti != null)
                        this.selectedPonuda.dokumenti.forEach(s => s.mode = FormMode.View);
                    this.izmijeniPonudu();
                }, error => {
                    this.toastr.error("Greška..");
                    console.error(error)
                });
            }, (reason) => {
            });

            modalRef.componentInstance.confirmText = "Da li ste sigurni da želite kopirati ponudu " + this.selectedPonuda.broj + " ?";
        }
    }
    selectedItem(item) {
        this.selectedPonuda.partner_sifra = item["item"]["sifra"];
        this.selectedPonuda.partner_naziv = item["item"]["naziv"];
        this.selectedPonuda.partner_adresa = item["item"]["adresa"];
        this.selectedPonuda.partner_email = item["item"]["email"];
        this.selectedPonuda.partner_telefon = item["item"]["telefon"];
        this.selectedPonuda.partner_jib = item["item"]["maticni_broj"];
    }
    cancel() {
        if (this.formMode == FormMode.Add || this.formMode == FormMode.Edit) {
            let modalRef = this.modalService.open(NgbdModalConfirm);
            modalRef.result.then((data) => {
                this.activeModal.close();
            }, (reason) => {
            });
            modalRef.componentInstance.confirmText = "Da li ste sigurni da želite zatvoriti prikaz? Sve nesačuvane izmjene će biti poništene.";
        }
        else
            this.activeModal.close();
    }
    search = (text$: Observable<string>) => {
        return text$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            // switchMap allows returning an observable rather than maps array
            switchMap((searchText) => this.searchPartner(searchText)),
            catchError(null)
        );
    }
    resultFormatPartnerListValue(value: any) {
        return value.sifra + "-" + value.naziv;
    }
    inputFormatPartnerListValue(value: any) {
        if (value.naziv)
            return value.naziv
        return value;

    }
    searchPartner(term: string) {
        if (term === '') {
            return of([]);
        }
        return this.http.get(this.baseUrl + 'partner/search?naziv=' + term)
            .pipe(
                map((response) => {
                    console.log(response);
                    return response;
                })
            );
    }
    reloadItem(continueAdd: boolean) {
        this.http.get<Ponuda>(this.baseUrl + 'ponuda/getbybroj?broj=' + this.selectedPonuda.broj).subscribe(result => {
            this.selectedPonuda = result;
            if (this.selectedPonuda.stavke != null)
                this.selectedPonuda.stavke.forEach(s => s.mode = FormMode.View);
            if (this.selectedPonuda.dokumenti != null)
                this.selectedPonuda.dokumenti.forEach(s => s.mode = FormMode.View);
            if (continueAdd == true)
                this.startItemAdd();
        }, error => {
            this.toastr.error("Greška..");
            console.error(error)
        });
    }

    dokument_download(dokument) {
        this.http.get(this.baseUrl + 'ponuda/dokument_download?naziv=' + dokument.naziv + '&broj=' + dokument.ponuda_broj
            , {
                responseType: 'arraybuffer'
            }
        ).subscribe(response => this.downLoadFile(response, dokument.opis));
        return false;
    }

    calculate(stavka) {
        stavka.vrijednost_nabavna = +(stavka.kolicina * stavka.cijena_nabavna).toFixed(2);
        stavka.ruc = +(stavka.vrijednost_nabavna * stavka.marza_procenat / 100).toFixed(2);
        if (stavka.cijena_nabavna != 0) {
            stavka.iznos_bez_pdv = +(stavka.vrijednost_nabavna + stavka.ruc).toFixed(2);
            stavka.cijena_bez_pdv = +(stavka.iznos_bez_pdv / stavka.kolicina).toFixed(2);
        }
        else {
            stavka.iznos_bez_pdv = +(stavka.kolicina * stavka.cijena_bez_pdv).toFixed(2);
        }
        stavka.rabat_iznos = +(stavka.iznos_bez_pdv * stavka.rabat_procenat / 100).toFixed(2);
        stavka.cijena_bez_pdv_sa_rabatom = +((stavka.iznos_bez_pdv - stavka.rabat_iznos) / stavka.kolicina).toFixed(2);
        stavka.iznos_bez_pdv_sa_rabatom = +(stavka.iznos_bez_pdv - stavka.rabat_iznos).toFixed(2);
        stavka.iznos_sa_pdv = +(stavka.iznos_bez_pdv_sa_rabatom * (1 + stavka.pdv_stopa / 100)).toFixed(2);
        stavka.cijena_sa_pdv = +(stavka.iznos_sa_pdv / stavka.kolicina).toFixed(2);
        stavka.pdv = +(stavka.iznos_sa_pdv - stavka.iznos_bez_pdv_sa_rabatom).toFixed(2);
    }
    calculate2(stavka) {
        stavka.iznos_bez_pdv = +(stavka.kolicina * stavka.cijena_bez_pdv).toFixed(2);
        stavka.ruc = stavka.vrijednost_nabavna == 0 ? 0 : (+(stavka.iznos_bez_pdv - stavka.vrijednost_nabavna).toFixed(2));
        stavka.marza_procenat = stavka.vrijednost_nabavna == 0 ? 0 : (+(stavka.ruc / stavka.vrijednost_nabavna * 100).toFixed(2));
        stavka.rabat_iznos = +(stavka.iznos_bez_pdv * stavka.rabat_procenat / 100).toFixed(2);
        stavka.cijena_bez_pdv_sa_rabatom = +((stavka.iznos_bez_pdv - stavka.rabat_iznos) / stavka.kolicina).toFixed(2);
        stavka.iznos_bez_pdv_sa_rabatom = +(stavka.iznos_bez_pdv - stavka.rabat_iznos).toFixed(2);
        stavka.iznos_sa_pdv = +(stavka.iznos_bez_pdv_sa_rabatom * (1 + stavka.pdv_stopa / 100)).toFixed(2);
        stavka.cijena_sa_pdv = +(stavka.iznos_sa_pdv / stavka.kolicina).toFixed(2);
        stavka.pdv = +(stavka.iznos_sa_pdv - stavka.iznos_bez_pdv_sa_rabatom).toFixed(2);
    }
    calculate3(stavka) {
        stavka.rabat_iznos = +(stavka.iznos_bez_pdv * stavka.rabat_procenat / 100).toFixed(2);
        stavka.cijena_bez_pdv_sa_rabatom = +((stavka.iznos_bez_pdv - stavka.rabat_iznos) / stavka.kolicina).toFixed(2);
        stavka.iznos_bez_pdv_sa_rabatom = +(stavka.iznos_bez_pdv - stavka.rabat_iznos).toFixed(2);
        stavka.iznos_sa_pdv = +(stavka.iznos_bez_pdv_sa_rabatom * (1 + stavka.pdv_stopa / 100)).toFixed(2);
        stavka.cijena_sa_pdv = +(stavka.iznos_sa_pdv / stavka.kolicina).toFixed(2);
        stavka.pdv = +(stavka.iznos_sa_pdv - stavka.iznos_bez_pdv_sa_rabatom).toFixed(2);
    }
    calculate4(stavka) {
        stavka.iznos_sa_pdv = +(stavka.iznos_bez_pdv_sa_rabatom * (1 + stavka.pdv_stopa / 100)).toFixed(2);
        stavka.cijena_sa_pdv = +(stavka.iznos_sa_pdv / stavka.kolicina).toFixed(2);
        stavka.pdv = +(stavka.iznos_sa_pdv - stavka.iznos_bez_pdv_sa_rabatom).toFixed(2);
    }

    startItemEdit(stavka: PonudaStavka) {
        stavka.mode = FormMode.Edit;
        setTimeout(_ => {
            this.naziv.find(s => s.nativeElement.id == stavka.stavka_broj).nativeElement.focus();
        }, 0);
    }
    cancelItemAdd(stavka: PonudaStavka) {
        this.selectedPonuda.stavke.splice(0, 1);
        stavka.mode = FormMode.View;
    }
    startItemAdd() {
        this.stavkeVisible = true;
        var newStavka = new PonudaStavka();
        newStavka.pdv_stopa = 17;
        newStavka.rabat_procenat = 0;
        newStavka.marza_procenat = 35;
        newStavka.jedinica_mjere = "KOM";
        newStavka.kolicina = 1;
        newStavka.cijena_nabavna = 0;
        newStavka.vrijednost_nabavna = 0;
        newStavka.ponuda_broj = this.selectedPonuda.broj;
        newStavka.mode = FormMode.Add;
        this.selectedPonuda.stavke = (new Array<PonudaStavka>(newStavka)).concat(this.selectedPonuda.stavke);
        setTimeout(_ => {
            this.naziv.find(s => s.nativeElement.id == "undefined").nativeElement.focus();
        }, 0);
    }
    deleteStavka(stavka: PonudaStavka) {
        let modalRef = this.modalService.open(NgbdModalConfirm);
        modalRef.result.then((data) => {
            this.http.get(this.baseUrl + 'ponuda/stavka_delete?ponuda_broj=' + stavka.ponuda_broj + "&stavka_broj=" + stavka.stavka_broj).subscribe(result => {
                console.log("OK");
                this.toastr.success("Stavka ponude je uspješno obrisana..");
                var ponuda = this.selectedPonuda;
                let index = ponuda.stavke.findIndex(d => d.stavka_broj === stavka.stavka_broj);
                ponuda.stavke.splice(index, 1);//remove element from array
                this.reloadItem(false);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        }, (reason) => {
        });
        modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati stavku ponude " + stavka.artikal_naziv + "-" + stavka.opis + " ?";
    }
    save(stavka: PonudaStavka, continueAdd: boolean) {
        if (stavka.mode == FormMode.Add)
            this.http.post<PonudaStavka>(this.baseUrl + 'ponuda/stavka_add', stavka).subscribe(result => {
                console.log("OK");
                this.toastr.success("Stavka ponude je uspješno dodata..");
                this.reloadItem(continueAdd);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        else
            this.http.put<PonudaStavka>(this.baseUrl + 'ponuda/stavka_update', stavka).subscribe(result => {
                console.log("OK");
                this.toastr.success("Stavka ponude je uspješno sačuvana..");
                this.reloadItem(false);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        stavka.mode = FormMode.View;
    }
    fileToUpload: File = null;
    handleFileInput(files: FileList, dokument) {
        this.fileToUpload = files.item(0);

        var oReq = new XMLHttpRequest();
        oReq.open("POST", this.baseUrl + 'ponuda/upload_dokument?broj=' + this.selectedPonuda.broj, true);
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
                dokument.mode = FormMode.View;
                this.reloadItem(false);
            }
            else if (oReq.readyState == 4 && oReq.status != 200) {
                this.toastr.error("Greška...");
                dokument.mode = FormMode.View;
                this.reloadItem(false);
            }
        };
    }
    saveDokument(dokument: PonudaDokument, continueAdd: boolean) {
        if (dokument.mode == FormMode.Add)
            this.http.post<PonudaDokument>(this.baseUrl + 'ponuda/dokument_add', dokument).subscribe(result => {
                console.log("OK");
                this.toastr.success("Dokument je uspješno dodat..");
                this.reloadItem(continueAdd);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        else
            this.http.put<PonudaDokument>(this.baseUrl + 'ponuda/dokument_update', dokument).subscribe(result => {
                console.log("OK");
                this.toastr.success("Dokument je uspješno sačuvan..");
                this.reloadItem(false);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        dokument.mode = FormMode.View;
    }
    startDokumentAdd() {
        this.dokumentiVisible = true;
        var newDokument = new PonudaDokument();
        newDokument.mode = FormMode.Add;
        newDokument.dokument_broj = this.selectedPonuda.broj;
        this.selectedPonuda.dokumenti = (new Array<PonudaDokument>(newDokument)).concat(this.selectedPonuda.dokumenti);

    }
    startDokumentEdit(dokument: PonudaDokument) {
        dokument.mode = FormMode.Edit;
    }
    cancelDokumentAdd(dokument: PonudaDokument) {
        this.selectedPonuda.dokumenti.splice(0, 1);
        dokument.mode = FormMode.View;
    }
    deleteDokument(dokument: PonudaDokument) {
        let modalRef = this.modalService.open(NgbdModalConfirm);
        modalRef.result.then((data) => {
            this.http.get(this.baseUrl + 'ponuda/dokument_delete?ponuda_broj=' + dokument.ponuda_broj + "&dokument_broj=" + dokument.dokument_broj).subscribe(result => {
                console.log("OK");
                this.toastr.success("Dokument je uspješno obrisan..");
                var ponuda = this.selectedPonuda;
                let index = ponuda.dokumenti.findIndex(d => d.dokument_broj === dokument.dokument_broj);
                ponuda.dokumenti.splice(index, 1);//remove element from array
                this.reloadItem(false);
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        }, (reason) => {
        });

        modalRef.componentInstance.confirmText = "Da li ste sigurni da želite obrisati dokument " + dokument.naziv + " ?";

    }
}




