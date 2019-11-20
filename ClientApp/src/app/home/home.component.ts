import { Component, Inject } from '@angular/core';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { Korisnik } from '../korisnik/korisnik.component';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import { Ponuda } from '../ponuda/ponuda.component';
import { environment } from '../../environments/environment';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
})
export class HomeComponent {
    currentUser: Korisnik;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService, @Inject('BASE_URL') public baseUrl: string
    ) {
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
    }

    logout() {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
    }

    ponude() {
        this.router.navigateByUrl('/ponude');
    }
    partneri() {
        this.router.navigateByUrl('/partneri');
    }
    korisnici() {
        this.router.navigateByUrl('/korisnici');
    }
    ugovori() {
        this.router.navigateByUrl('/ugovori');
    }
    servisneprijave() {
        this.router.navigateByUrl('/servisneprijave');
    }
    izvjestaji() {
        this.router.navigateByUrl('/izvjestaji');
    }
    selectedPonuda: Ponuda;
    pdf() {
        this.selectedPonuda = { "broj": "00001/2019", "status": "E", "predmet": "werwer", "radnik": "dario", "datum": new Date("2019-10-07T00:00:00"), "partner_sifra": 887, "partner_jib": "401301100000", "partner_adresa": "Cara Dušana bb Celinac 78240", "partner_naziv": "Argentina presente", "partner_telefon": "065/511-000", "partner_email": "fama@teol.net", "valuta_placanja": "wer", "rok_vazenja": "324", "paritet_kod": null, "paritet": "CIP - Transport i osiguranje plaćeni (do naznačene destinacije)", "rok_isporuke": "23", "iznos_bez_rabata": 3179.250000, "rabat": 0.000000, "iznos_sa_rabatom": 3179.250000, "pdv": 540.470000, "iznos_sa_pdv": 3719.720000, "nabavna_vrijednost": 2355.000000, "ruc": 824.250000, "partner": { "sifra": 887, "naziv": "Argentina presente", "tip": "P", "maticni_broj": "401301100000", "adresa": "Cara Dušana bb Celinac 78240", "telefon": "065/511-000", "email": "dario.djekic@lanaco.com", "kupac": false, "dobavljac": false, "broj_lk": null }, "korisnik": { "korisnicko_ime": "dario", "ime": "Dario", "prezime": "Đekić", "email": "dariodjekic@gmail.com", "lozinka": "8a49317e060e23bb86f9225ca581e0a9", "admin": true, "token": null }, "stavke": [{ "ponuda_broj": "00001/2019", "stavka_broj": 2, "artikal_naziv": "qweq", "opis": "eqweqwe", "jedinica_mjere": "KOM", "kolicina": 1.000000, "cijena_bez_pdv": 3136.050000, "cijena_bez_pdv_sa_rabatom": 3136.050000, "rabat_procenat": 0.000000, "rabat_iznos": 0.000000, "iznos_bez_pdv": 3136.050000, "iznos_bez_pdv_sa_rabatom": 3136.050000, "cijena_nabavna": 2323.000000, "vrijednost_nabavna": 2323.000000, "marza_procenat": 35.000000, "ruc": 813.050000, "pdv_stopa": 17.000000, "pdv": 533.130000, "cijena_sa_pdv": 3669.180000, "iznos_sa_pdv": 3669.180000 }, { "ponuda_broj": "00001/2019", "stavka_broj": 3, "artikal_naziv": "weqw", "opis": "qwe", "jedinica_mjere": "KOM", "kolicina": 1.000000, "cijena_bez_pdv": 43.200000, "cijena_bez_pdv_sa_rabatom": 43.200000, "rabat_procenat": 0.000000, "rabat_iznos": 0.000000, "iznos_bez_pdv": 43.200000, "iznos_bez_pdv_sa_rabatom": 43.200000, "cijena_nabavna": 32.000000, "vrijednost_nabavna": 32.000000, "marza_procenat": 35.000000, "ruc": 11.200000, "pdv_stopa": 17.000000, "pdv": 7.340000, "cijena_sa_pdv": 50.540000, "iznos_sa_pdv": 50.540000 }] };

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
                    { columns: [{ text: 'Datum: ', bold: true, width: 60 }, { text: this.selectedPonuda.datum.toLocaleDateString('sr-Latn-ba'), width: 250 }] },
                    { columns: [{ text: 'Partner: ', bold: true, width: 60 }, { text: this.selectedPonuda.partner_naziv, width: 250 }]},
                    { columns: [{ text: 'JIB: ', bold: true, width: 60 }, { text: this.selectedPonuda.partner_jib, width: 250 }] },
                    { columns: [{ text: 'Adresa: ', bold: true, width: 60 }, { text: this.selectedPonuda.partner_adresa, width: 250 }] }

                ],
                [
                    { columns: [{ text: 'Valuta: ', bold: true, width: 95 }, { text: this.selectedPonuda.valuta_placanja, width: 'auto' }] },
                    { columns: [{ text: 'Opcija ponude: ', bold: true, width: 95 }, { text: this.selectedPonuda.rok_vazenja, width: 'auto' }] },
                    { columns: [{ text: 'Rok isporuke: ', bold: true, width: 95 }, { text: this.selectedPonuda.rok_isporuke, width: 'auto'}] },
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
            text: "Dokument sastavio: ", alignment: 'right',bold:true
        });
        dd.content.push({
            text: this.selectedPonuda.korisnik.ime + " " + this.selectedPonuda.korisnik.prezime, alignment: 'right'
        });
        dd.footer = { width: 510, image: environment.footer,alignment:'center' };
        pdfMake.vfs = pdfFonts.pdfMake.vfs;
        var pdf = pdfMake.createPdf(dd);
        pdf.download();
        //pdf.getBlob((b) => {
        //    var oReq = new XMLHttpRequest();
        //    oReq.open("POST", this.baseUrl + 'ponuda/uploadPDF?broj=' + this.selectedPonuda.broj, true);
        //    oReq.onload = function (oEvent) {
        //        // Uploaded.
        //    };
        //    var form = new FormData();
        //    form.append("blob", b, this.selectedPonuda.broj.replace('/', '_') + ".pdf");

        //    oReq.send(form);
        //    oReq.onreadystatechange = function () {
        //        if (this.readyState == 4 && this.status == 200) {
        //            let blob = new Blob([this.response], { type: 'message/rfc822' });
        //            let url = window.URL.createObjectURL(blob);
        //            let pwa = window.open(url);
        //            if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
        //                alert('Please disable your Pop-up blocker and try again.');
        //            }
        //        }
        //    };

        //});

    }




}
