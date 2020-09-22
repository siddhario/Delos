import { Component, OnInit, ElementRef, ViewChild, AfterViewInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
}) export class AppComponent implements OnInit, AfterViewInit {
  //p: ElementRef<any>;
  ngAfterViewInit(): void {
    // this.p = this.p1;
    this.txt = ((new Date()).getMonth()+1).toString().padStart(2, '0')+"."
      + (new Date()).getDate().toString().padStart(2, '0')+"."
      + (new Date()).getFullYear().toString().substring(0, 2)
      + " SYSTEM INITIATED."
      +" BUILD 0.0.6"
      ;
    this.typeWriter(this);
  }
  i = 0;
  txt = '05.03.20. SYSTEM INITIATED. BUILD 0.0.6.';
  speed = 50;

  typeWriter(that) {
    if (that.i < that.txt.length) {
      that.p1.nativeElement.innerHTML += that.txt.charAt(that.i);
      that.i++;
      setTimeout(that.typeWriter, that.speed, that);
    }
    else {
      setTimeout(that.close, 5000, that);
    }
  }
  close(that) {
    that.p1.nativeElement.style.display = "none";
  }


  @ViewChild('p1', { static: false }) p1: ElementRef<any>;
  ngOnInit(): void {
  }
  title = 'app';
}
