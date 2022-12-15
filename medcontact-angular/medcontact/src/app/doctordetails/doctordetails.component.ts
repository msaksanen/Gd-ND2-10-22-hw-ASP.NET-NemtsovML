import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Doctorfulldata } from '../models/doctorfulldata';

@Component({
  selector: 'app-doctordetails',
  templateUrl: './doctordetails.component.html',
  styleUrls: ['./doctordetails.component.scss']
})
export class DoctordetailsComponent {
  @Input() DetailDoctor?: Doctorfulldata;


}
