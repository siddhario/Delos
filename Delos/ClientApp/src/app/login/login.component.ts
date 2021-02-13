import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { AuthenticationService } from '../auth/auth.service';



@Component({ templateUrl: 'login.component.html' })
export class LoginComponent implements OnInit {
    loginForm: FormGroup;
    loading = false;
    submitted = false;
    returnUrl: string;
    error = '';
    username: string;
    password: string;


    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthenticationService,
        private formBuilder: FormBuilder,
    ) {
        // redirect to home if already logged in
        if (this.authenticationService.currentUserValue) {
            this.router.navigate(['/']);
        }

        this.loginForm = this.formBuilder.group({
            username: '',
            password: ''
        });
    }
    onSubmit(loginData) {
        this.login(loginData);
        //this.loginForm.reset();
    }

    ngOnInit() {
      
        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

 

    login(loginData) {
   

        this.loading = true;
        this.authenticationService.login(loginData.username, loginData.password)
            .pipe(first())
            .subscribe(
                data => {
                    this.router.navigate([this.returnUrl]);
                },
                error => {
                    this.error = error.error;
                    this.loading = false;
                });
    }
}
