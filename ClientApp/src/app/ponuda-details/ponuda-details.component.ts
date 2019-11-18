import { Component, Inject, OnInit, ViewChild } from '@angular/core';
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
import { Korisnik } from '../korisnik/korisnik.component';
import { AuthenticationService } from '../auth/auth.service';

@Component({
    selector: 'app-ponuda-details',
    templateUrl: './ponuda-details.component.html'
})


export class PonudaDetailsComponent implements OnInit {

    public model: any;

    makeDocument() {
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
                    { columns: [{ text: 'Datum: ', bold: true, width: 60 }, { text: (new Date(this.selectedPonuda.datum)).toLocaleDateString('sr-Latn-ba'), width: 250 }] },
                    { columns: [{ text: 'Partner: ', bold: true, width: 60 }, { text: this.selectedPonuda.partner_naziv, width: 250 }] },
                    { columns: [{ text: 'JIB: ', bold: true, width: 60 }, { text: this.selectedPonuda.partner_jib, width: 250 }] },
                    { columns: [{ text: 'Adresa: ', bold: true, width: 60 }, { text: this.selectedPonuda.partner_adresa, width: 250 }] }

                ],
                [
                    { columns: [{ text: 'Valuta: ', bold: true, width: 95 }, { text: this.selectedPonuda.valuta_placanja, width: 'auto' }] },
                    { columns: [{ text: 'Opcija ponude: ', bold: true, width: 95 }, { text: this.selectedPonuda.rok_vazenja, width: 'auto' }] },
                    { columns: [{ text: 'Rok isporuke: ', bold: true, width: 95 }, { text: this.selectedPonuda.rok_isporuke, width: 'auto' }] },
                    { columns: [{ text: 'Paritet: ', bold: true, width: 95 }, { text: this.selectedPonuda.paritet_kod, width: 'auto' }] }
                ]
            ]
        });

        dd.content.push('\n');

        var itemsTable = {
            style: 'tableExample', table: {
                headerRows: 1,
                widths: [200, 50, 40, 60, 40, 60], body: [
                ]
            },
            layout: 'lightHorizontalLines'
        };

        itemsTable.table.body.push([
            {
                text: [
                    { text: 'Naziv artikla' + '\n', bold: true },
                    { text: 'Opis', fontSize: 10 }]
            },
            { text: 'Količina', style: 'tableHeader' },
            { text: 'JM', style: 'tableHeader' },
            { text: 'Cijena bez PDV-a', style: 'tableHeader' },
            { text: 'Rabat', style: 'tableHeader' },
            { text: 'Cijena sa PDV-om', style: 'tableHeader' }]);

        this.selectedPonuda.stavke.forEach(s => {
            itemsTable.table.body.push([
                {
                    text: [
                        { text: s.artikal_naziv + '\n', bold: true },
                        { text: s.opis, fontSize: 10 }]
                },
                { text: s.kolicina, alignment: 'right' },
                { text: s.jedinica_mjere, alignment: 'center' },
                { text: s.cijena_bez_pdv.toFixed(2), alignment: 'right' },
                { text: s.rabat_iznos.toFixed(2), alignment: 'right' },
                { text: s.iznos_bez_pdv.toFixed(2), alignment: 'right' }]
            );
        });

        dd.content.push(itemsTable);

        dd.content.push(
            {
                columns: [{ text: "Ukupan iznos bez rabata:", bold: true, alignment: 'right', width: 450 }, { text: this.selectedPonuda.iznos_bez_rabata.toFixed(2), width: 80, alignment: 'right' }]
            },
            {
                columns: [{ text: "Iznos rabata:", bold: true, alignment: 'right', width: 450 }, { text: this.selectedPonuda.rabat.toFixed(2), width: 80, alignment: 'right' }]
            },
            {
                columns: [{ text: "Ukupan iznos bez PDV-a:", bold: true, alignment: 'right', width: 450 }, { text: this.selectedPonuda.iznos_sa_rabatom.toFixed(2), width: 80, alignment: 'right' }]
            },
            {
                columns: [{ text: "PDV:", bold: true, alignment: 'right', width: 450 }, { text: this.selectedPonuda.pdv.toFixed(2), width: 80, alignment: 'right' }]
            },
            {
                columns: [{ text: "Ukupan iznos sa PDV-om:", bold: true, alignment: 'right', width: 450 }, { text: this.selectedPonuda.iznos_sa_pdv.toFixed(2), width: 80, alignment: 'right' }]
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
        return dd;
    }

    pdf() {
        var dd = this.makeDocument();

        pdfMake.vfs = pdfFonts.pdfMake.vfs;
        var pdf = pdfMake.createPdf(dd);
        pdf.download();
    }

    email() {
        var dd = this.makeDocument();
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

            oReq.send(form);
            oReq.onreadystatechange = function () {
                if (this.readyState == 4 && this.status == 200) {
                    let blob = new Blob([this.response], { type: 'message/rfc822' });
                    let url = window.URL.createObjectURL(blob);
                    let pwa = window.open(url);
                    if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
                        alert('Please disable your Pop-up blocker and try again.');
                    }
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

    /**
     * Method is use to download file.
     * @param data - Array Buffer data
     * @param type - type of the document.
     */
    downLoadFile(data: any, type: string) {
        let blob = new Blob([data], { type: type });
        let url = window.URL.createObjectURL(blob);
        let pwa = window.open(url);
        if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
            alert('Please disable your Pop-up blocker and try again.');
        }
    }

    //email() {
    //    if (this.selectedPonuda != undefined) {
    //        //let modalRef = this.modalService.open(NgbdModalConfirm);
    //        //modalRef.result.then((data) => {
    //        this.http.get(this.baseUrl + 'ponuda/email?broj=' + this.selectedPonuda.broj
    //            , {
    //                responseType: 'arraybuffer'
    //            }
    //        ).subscribe(response => this.downLoadFile(response, "message/rfc822"));
    //        //}, (reason) => {
    //        //});

    //        //modalRef.componentInstance.confirmText = "Da li ste sigurni da želite zaključiti ponudu " + this.selectedPonuda.broj + " ?";
    //    }
    //}

    excel() {
        if (this.selectedPonuda != undefined) {
            //let modalRef = this.modalService.open(NgbdModalConfirm);
            //modalRef.result.then((data) => {
            this.http.get(this.baseUrl + 'ponuda/excel?broj=' + this.selectedPonuda.broj
                , {
                    responseType: 'arraybuffer'
                }
            ).subscribe(response => this.downLoadFile(response, "application/vnd.ms-excel.sheet.macroEnabled.12"));
            //}, (reason) => {
            //});

            //modalRef.componentInstance.confirmText = "Da li ste sigurni da želite zaključiti ponudu " + this.selectedPonuda.broj + " ?";
        }
    }


    statusiraj(status) {
        if (this.selectedPonuda != undefined) {
            let modalRef = this.modalService.open(NgbdModalConfirm);
            modalRef.result.then((data) => {
                this.http.put<Ponuda>(this.baseUrl + 'ponuda/statusiraj?broj=' + this.selectedPonuda.broj + "&status=" + status, null).subscribe(result => {
                    this.toastr.success("Status ponude je uspješno postavljen..");
                    this.selectedPonuda = result;
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
                }, error => {
                    this.toastr.error("Greška..");
                    console.error(error)
                });
            }, (reason) => {
            });

            modalRef.componentInstance.confirmText = "Da li ste sigurni da želite kopirati ponudu " + this.selectedPonuda.broj + " ?";
        }
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

    selectedItem(item) {
        this.selectedPonuda.partner_sifra = item["item"]["sifra"];
        this.selectedPonuda.partner_naziv = item["item"]["naziv"];
        this.selectedPonuda.partner_adresa = item["item"]["adresa"];
        this.selectedPonuda.partner_email = item["item"]["email"];
        this.selectedPonuda.partner_telefon = item["item"]["telefon"];
        this.selectedPonuda.partner_jib = item["item"]["maticni_broj"];
    }
    /**
 * Used to format the result data from the lookup into the
 * display and list values. Maps `{name: "band", id:"id" }` into a string
*/
    resultFormatPartnerListValue(value: any) {
        return value.sifra + "-" + value.naziv;
    }
    /**
      * Initially binds the string value and then after selecting
      * an item by checking either for string or key/value object.
    */
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

    ngOnInit(): void {
        if (this.selectedPonuda != undefined) {
            this.http.get<PonudaStavka[]>(this.baseUrl + 'ponuda_stavka?ponuda_broj=' + this.selectedPonuda.broj).subscribe(result => {
                this.selectedPonuda.stavke = result;
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
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

    savePonuda(ponuda) {
        if ((typeof ponuda.partner) == "string") {
            ponuda.partner_naziv = ponuda.partner;
            ponuda.partner = new partner();
            ponuda.partner.naziv = ponuda.partner_naziv;
        }
        if (ponuda.broj == undefined) {
            let obj: object = ponuda.datum;

            //if ((typeof obj) == "object")
            //    ponuda.datum = new Date(obj["year"], obj["month"], obj["day"]);
            //let obj: Date = ponuda.datum;
            //ponuda.datum = obj.toDateString();
            this.http.post<Ponuda>(this.baseUrl + 'ponuda', ponuda).subscribe(result => {
                console.log("OK");
                this.toastr.success("Ponuda je uspješno dodata..");
                //stavka.editing = false;
                ponuda = result;
                this.selectedPonuda = ponuda;

                //this.reloadItem(continueAdd);
                //this.itemAdd = false;
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        } else {
            //ponuda.datum = new Date(ponuda.datum.getFullYear(), ponuda.datum.getMonth(), ponuda.datum.getDate());
            this.http.put<Ponuda>(this.baseUrl + 'ponuda', ponuda).subscribe(result => {
                console.log("OK");
                this.toastr.success("Ponuda je uspješno sačuvana..");
                ponuda = result;
                //this.reloadItem(false);
                //stavka.editing = false;
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
            //this.activeModal.close();
        }
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

    reloadItem(continueAdd: boolean) {
        this.http.get<Ponuda>(this.baseUrl + 'ponuda/getbybroj?broj=' + this.selectedPonuda.broj).subscribe(result => {
            this.selectedPonuda = result;
            if (continueAdd == true)
                this.startItemAdd();
        }, error => {
            this.toastr.error("Greška..");
            console.error(error)
        });
    }
    cancel() {
        this.activeModal.close();
    }

    save(stavka: PonudaStavka, continueAdd: boolean) {
        //stavka.cijena_nabavna = +stavka.cijena_nabavna;
        //stavka.marza_procenat = +stavka.marza_procenat;
        //stavka.kolicina = +stavka.kolicina;
        //stavka.rabat_procenat = +stavka.rabat_procenat;
        //stavka.cijena_sa_pdv = +stavka.cijena_sa_pdv;
        //stavka.pdv_stopa = +stavka.pdv_stopa;
        //stavka.cijena_bez_pdv = +stavka.cijena_bez_pdv;
        //stavka.cijena_bez_pdv_sa_rabatom = +stavka.cijena_bez_pdv_sa_rabatom;
        if (stavka.stavka_broj == undefined)
            this.http.post<PonudaStavka>(this.baseUrl + 'ponuda/stavka_add', stavka).subscribe(result => {
                console.log("OK");
                stavka.editing = false;
                this.toastr.success("Stavka ponude je uspješno dodata..");
                this.reloadItem(continueAdd);
                this.itemAdd = false;
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });
        else
            this.http.put<PonudaStavka>(this.baseUrl + 'ponuda/stavka_update', stavka).subscribe(result => {
                console.log("OK");
                this.toastr.success("Stavka ponude je uspješno sačuvana..");
                this.reloadItem(false);
                stavka.editing = false;
            }, error => {
                this.toastr.error("Greška..");
                console.error(error)
            });


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

    constructor(private authenticationService: AuthenticationService, private modalService: NgbModal, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public activeModal: NgbActiveModal, private toastr: ToastrService) {
        this.startAdd();
        toastr.toastrConfig.positionClass = "toast-bottom-right";
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
    }

    startAdd() {
        this.selectedPonuda = new Ponuda();
        this.selectedPonuda.status = "E";
        //this.selectedPonuda.radnik = this.currentUser.korisnicko_ime;
        this.selectedPonuda.datum = (new Date());
        this.selectedPonuda.rok_isporuke = "3-5 dana";
        this.selectedPonuda.rok_vazenja = "7 dana";
        this.selectedPonuda.valuta_placanja = "avans";
    }

    currentUser: Korisnik;
}

@Pipe({
    name: 'noComma'
})
export class NoCommaPipe implements PipeTransform {

    transform(val: number): string {
        if (val !== undefined && val !== null) {
            // here we just remove the commas from value
            return val.toString().replace(/,/g, "");
        } else {
            return "";
        }
    }
}

class partner {
    sifra: number;
    naziv: string;
    tip: string;
    email: string;
    adresa: string;
    telefon: string;
    kupac: boolean;
    dobavljac: boolean;
    broj_lk: string;
    selected: boolean;
}


export class Ponuda {
    broj: string;
    datum: Date;
    selected: boolean;
    partner: partner;
    partner_sifra: number;
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
    korisnik: Korisnik;
    rabat: number;
    iznos_bez_rabata: number;
}



export class PonudaStavka {
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
    editing?: boolean;
}
