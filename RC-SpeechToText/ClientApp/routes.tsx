import * as React from 'react';
import { Route } from 'react-router-dom';
import { Home } from './components/Home';
import Navbar from './components/Navbar/Navbar';
import TestAccuracy from './components/Google/TestAccuracy';
import Transcription from './components/Google/Transcription'; 
import VideoTable from './components/MyVideos/VideoTable'


export const routes = <div>
    <Route path='/' component = { Navbar } />
    <Route exact path='/' component={Home} />
    <Route exact path="/TestAccuracy" component={TestAccuracy} />
    <Route exact path="/Transcription" component={Transcription} />
    <Route exact path="/videos" component={ VideoTable } />
</div>;
