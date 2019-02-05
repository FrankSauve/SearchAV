import * as React from 'react';
import { Route } from 'react-router-dom';
import { Home } from './components/Home';
import Unauthorized from './components/Unauthorized';
import Navbar from './components/Navbar/Navbar';
import Dashboard from './components/Dashboard/Dashboard';
import FileView from './components/FileView/FileView';
import MyFiles from './components/Filters/MyFiles';


export const routes = <div>
    <Route path='/' component={ Navbar } />
    <Route exact path="/unauthorized" component={Unauthorized} />
    <Route exact path='/' component={Home} />
    <Route exact path="/dashboard" component={Dashboard} />
    <Route exact path="/dashboard/myFiles/:id" component={MyFiles} />
    <Route exact path="/FileView/:id" component={FileView} />
</div>;
