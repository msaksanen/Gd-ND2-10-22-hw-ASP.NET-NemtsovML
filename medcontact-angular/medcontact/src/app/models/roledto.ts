import { Acsdatadto } from "./acsdatadto";
import { Customerdatadto } from "./customerdatadto";
import { Doctordatadto } from "./doctordatadto";
import { Userdto } from "./userdto";

export class Roledto {
  id?: string;
  name?: string;
  isSelected?: boolean;
  users?: Array<Userdto>;
  doctorDatas?: Array<Doctordatadto>;
  customerData?: Array<Customerdatadto>;
  acsDatas?: Array<Acsdatadto>;
}
