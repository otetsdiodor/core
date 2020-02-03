/* eslint-disable default-case */
/* eslint-disable jsx-a11y/anchor-is-valid */
/* eslint-disable eqeqeq */
import React, { Component } from 'react';

export class Statistic extends Component {
  constructor(props) {
    super(props);
    this.state = {
      forecasts: [],
      loading: true,
      Status:-1,
      nickname:"",
      countofships:"",
      countofsteps:"",
      date:"",
      isLoaded: true
    };
  }

  componentDidMount() {
    console.log("FROM DIDMOUNT______")
    this.getStatistic();
  }

  renderForecastsTable = (forecasts) =>  {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th><a onClick={() => this.SortClick('name')}>Имя</a></th>
            <th><a onClick={() => this.SortClick('ships')}>Кол-во кораблей</a></th>
            <th><a onClick={() => this.SortClick('count')}>Кол-во шагов</a></th>
            <th><a onClick={() => this.SortClick('date')}>Дата</a></th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map(forecast =>
            <tr key={forecast.date}>
              <td>{forecast.winnerName}</td>
              <td>{forecast.shipsInfo.length}</td>
              <td>{forecast.countOfSteps}</td>
              <td>{forecast.endTime}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  SortClick = (name) =>{
    let status;
    switch (name) {
      case "name":
          if (this.state.Status == 0) status = 1;
          else status = 0;
          break;
      case "ships":
          if (this.state.Status == 2) status = 3;
          else status = 2;
          break;
      case "count":
          if (this.state.Status == 4) status = 5;
          else status = 4;
          break;
       case "date":
          if (this.state.Status == 6) status = 7;
          else status = 6;
          break;
    }
    this.setState({
      Status:status,
      isLoaded:false
    });
  }


  async getStatistic() {
  
    let params = {
      "Name": this.state.nickname,
      "CountShips": this.state.countofships,
      "CountSteps": this.state.countofsteps,
      "date": this.state.date,
      "State": this.state.Status
    };
    let query = Object.keys(params)
            .map(k => encodeURIComponent(k) + '=' + encodeURIComponent(params[k]))
            .join('&');
    
    const response = await fetch('api/statistic?'+query);
    const data = await response.json();
    this.setState({ 
      forecasts: data,
      loading: false,
      isLoaded:true
      });
  }

  filterForm = () =>  {
    return (
     <form>
       <label for="nickname">Ник:</label>
       <input id="nickname" type="text" name="nickname" onChange={this.handleInputChange}/>
       
       <label for="nickname">Кол-во кораблей:</label>
       <input id="nickname" type="text" name="countofships" onChange={this.handleInputChange}/>
       
       <label for="nickname">Кол-во шагов:</label>
       <input id="nickname" type="text" name="countofsteps" onChange={this.handleInputChange}/>
       
       <label for="nickname">Дата:</label>
       <input id="nickname" type="text" name="date" onChange={this.handleInputChange}/>

     </form>
    );
  }

  handleInputChange = (event) => {
    const target = event.target;
    const value = target.value;
    const name = target.name;

    this.setState({
      [name]: value,
      isLoaded:false
    });
  }

  render() {
    if(!this.state.isLoaded)
    {
      this.getStatistic();
    }
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderForecastsTable(this.state.forecasts);
    let FilterForm = this.filterForm();
    return (
      <div className="container">
        <h1>Статистика игр</h1>
        {FilterForm}
        {contents}
      </div>
    );
  }
}
