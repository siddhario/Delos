import { Component, Inject } from '@angular/core';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { Korisnik } from '../model/korisnik';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';

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
        this.authenticationService.currentUser.subscribe(x => {
            this.currentUser = x;
            if (this.currentUser == null)
                this.router.navigate(['/login']);
        });
        return;
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

        dd.content.push({
            table: {
                widths: [310],
                body:
                    [
                        [
                            { fillColor: '#eeeeee', text: 'ORGANIZACIJA', fontSize: 9, bold: true, border: [true, true, true, false] }
                        ]
                    ]
            }
        });
        dd.content.push({
            table: {
                widths: [510],
                body:
                    [
                        [
                            { text: 'IZDATNICA BROJ 2', alignment: 'center', fontSize: 13, bold: true, border: [true, true, true, false] }
                        ]
                    ]
            }
        });
        dd.content.push({
            table: {
                widths: [380, 120],
                body:
                    [
                        [
                            {
                                stack: [
                                    {
                                        border: [true, true, true, true], columns: [
                                            { text: 'Poslovnica:', alignment: 'left', fontSize: 9, bold: true, width: 60 },
                                            { text: '[02]-Materijal', alignment: 'left', fontSize: 9 }
                                        ]
                                    },
                                    {
                                        border: [true, true, true, true], columns: [
                                            { text: 'Mjesto troška:', alignment: 'left', fontSize: 9, bold: true, width: 60 },
                                            { text: '[111]-Klinika', alignment: 'left', fontSize: 9 }
                                        ]
                                    },
                                    {
                                        border: [true, true, true, true], columns: [
                                            { text: 'Radnik:', alignment: 'left', fontSize: 9, bold: true, width: 60 },
                                            { text: 'Edita Aradinović', alignment: 'left', fontSize: 9 }
                                        ]
                                    }
                                ]
                            },
                            {
                                stack: [
                                    {
                                        border: [true, true, true, true], columns: [
                                            { text: 'Datum:', alignment: 'left', fontSize: 9, bold: true, width: 40 },
                                            { text: '12.09.2019', alignment: 'left', fontSize: 9 }
                                        ]
                                    },
                                    {
                                        border: [true, true, true, true], columns: [
                                            { text: 'Status:', alignment: 'left', fontSize: 9, bold: true, width: 40 },
                                            { text: 'Evidentirana', alignment: 'left', fontSize: 9 }
                                        ]
                                    }
                                ]
                            }

                        ]
                    ]
            }
        });

        pdfMake.vfs = pdfFonts.pdfMake.vfs;
        var pdf = pdfMake.createPdf(dd);
        pdf.download();


    }

    logout() {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
    }
    webShopServisi() {
        this.router.navigateByUrl('/servisi');
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
}
