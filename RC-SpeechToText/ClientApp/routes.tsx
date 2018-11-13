import * as React from 'react';
import { Route } from 'react-router-dom';
import { Home } from './components/Home';
import Navbar from './components/Navbar/Navbar';
import Google from './components/Google/Google';
import { GetVideo } from './GetVideo';
import { AddVideo } from './AddVideo';

export const routes = <div>
    <Route path='/' component = { Navbar } />
    <Route exact path='/' component={ Home } />
    <Route exact path="/google" component = { Google } />
    <Route exact path="/videos" component={GetVideo} />
    <Route exact path="/videos" component={AddVideo} />
</div>;
