import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { KorisnikComponent } from './korisnik/korisnik.component';
import { PonudaComponent, FilterPipe } from './ponuda/ponuda.component';
import { PonudaDetailsComponent } from './ponuda-details/ponuda-details.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        CounterComponent,
        FetchDataComponent,
        KorisnikComponent,
        PonudaComponent,
        PonudaDetailsComponent,
        FilterPipe
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        NgbModule,
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'korisnici', component: KorisnikComponent },
            { path: 'ponude', component: PonudaComponent },
        ])
    ],
    entryComponents: [PonudaDetailsComponent],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
