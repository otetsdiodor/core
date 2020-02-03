import React, { Component } from 'react';
import { Switch, Route } from 'react-router-dom'
import { Game } from './components/Game';
import { Statistic } from './components/Statistic';
import './custom.css'

export default class App extends Component {
  static displayName = App.name;

    render() {
        return (
            <Switch>
                <Route path='/index/stat' component={Statistic}/>
                <Route path='/index/game' component={Game}/>
            </Switch>
            // <Layout>
            //     {/* <Route path='/index' component={Game}/> */}
            //     <Route path='/index' component={Statistic}/>
            // </Layout>
        );
    }
}
