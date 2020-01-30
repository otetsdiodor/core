import { Component, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FormBuilder } from '@angular/forms';
import { ReactiveFormsModule  } from '@angular/forms'

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public Statistics: ResultStats[];
  Status: number;
  Http: HttpClient;
  Url: string;
  FilterForm;
  Params;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, formBuilder: FormBuilder) {
    this.Http = http;
    this.Url = baseUrl + 'statistic'
    this.Status = -1;
    this.GetData();
    this.FilterForm = formBuilder.group({
      nickname: '',
      countofships:'',
      countofsteps:'',
      date:'',
    })
  }

  GetData() {
    let params = new HttpParams().set("State", this.Status.toString());
    if (this.Params != null) {
      params = new HttpParams()
        .set("Name", this.Params.nickname)
        .set("CountShips", this.Params.countofships)
        .set("countSteps", this.Params.countofsteps)
        .set("date", this.Params.date)
        .set("State", this.Status.toString());
    }
    this.Http.get<ResultStats[]>(this.Url, { params: params }).subscribe(result => {
      this.Statistics = result;
    }, error => console.error(error));
  }

  onSubmit(FilterData) {
    this.Params = FilterData;
    this.GetData();
  }

  SortClick(type) {
    switch (type) {
      case "name":
        if (this.Status == 0) {
          this.Status = 1;
        }
        else {
          this.Status = 0;
        }
        break;
      case "ships":
        if (this.Status == 2) {
          this.Status = 3;
        }
        else {
          this.Status = 2;
        }
        break;
      case "count":
        if (this.Status == 4) {
          this.Status = 5;
        }
        else {
          this.Status = 4;
        }
        break;
      case "date":
        if (this.Status == 6) {
          this.Status = 7;
        }
        else {
          this.Status = 6;
        }
        break;
    }
    this.GetData();
  }

}

interface ResultStats {
  winnerName: string;
  countOfSteps: number;
  shipsInfo: [];
  endTime: Date;
}
