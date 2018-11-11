import * as React from 'react';
import { Route } from 'react-router-dom';
import { Home } from './components/Home';
import Navbar from './components/Navbar/Navbar';
import Google from './components/Google/Google';


export const routes = <div>
    <Route path='/' component = { Navbar } />
    <Route exact path='/' component={ Home } />
    <Route exact path="/google" component = { Google } />
</div>;
