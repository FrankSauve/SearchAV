import * as React from 'react';
import { Route } from 'react-router-dom';
import { Home } from './components/Home';
import Unauthorized from './components/Unauthorized';
import Navbar from './components/Navbar/Navbar';
import Dashboard from './components/Dashboard/Dashboard';


export const routes = <div>
    <Route path='/' component={ Navbar } />
    <Route exact path="/unauthorized" component={Unauthorized} />
    <Route exact path='/' component={Home} />
    <Route exact path="/Dashboard" component={Dashboard} />
</div>;
