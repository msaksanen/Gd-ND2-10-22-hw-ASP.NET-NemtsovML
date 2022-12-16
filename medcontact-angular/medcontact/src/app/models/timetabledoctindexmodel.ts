import { Daytimetabledto } from "./daytimetabledto";
import { Doctorinfo } from "./doctorinfo";
import { Pageviewmodel } from "./pageviewmodel";
import { Sortviewmodel } from "./sortviewmodel";
import { Userdto } from "./userdto";

export class Timetabledoctindexmodel {
  doctorInfo?: Doctorinfo;
  tableList?: Array<Daytimetabledto>;
  pageViewModel?: Pageviewmodel;
  sortViewModel?: Sortviewmodel;
  flag?: number;
  user?: Userdto;
  processOptions?: string;
  reflink?: string;
}
