import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { Korisnik } from '../korisnik/korisnik.component';
import { AuthenticationService } from '../auth/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
    }

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
