import { Customerdatadto } from "./customerdatadto";
import { Doctordatadto } from "./doctordatadto";
import { Filedatadto } from "./filedatadto";

export class Meddatadto {
  id?: string;
  inputDate?: Date;
  department?: string;
  type?: string;
  shortSummary?: string;
  textData?: string;
  fileDatas?: Array<Filedatadto>;
  customerDataId?: string;
  customerData?: Customerdatadto;
  doctorDataId?: string;
  doctorData?: Doctordatadto;
}
