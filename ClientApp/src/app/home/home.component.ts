import { Component, Inject } from '@angular/core';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { Korisnik } from '../korisnik/korisnik.component';
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
        this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
    }

    logout() {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
    }  

    

}
