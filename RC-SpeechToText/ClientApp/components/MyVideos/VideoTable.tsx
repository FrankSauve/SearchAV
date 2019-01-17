import * as React from 'react';
import axios from 'axios';
import Video from './Video';
import { RouteComponentProps } from 'react-router';
import { Redirect } from 'react-router-dom'
import auth from '../../Utils/auth';

interface State {
    videos: any[]
    loading: boolean,
    unauthorized: boolean
}

export default class VideoTable extends React.Component<RouteComponentProps<{}>, State> {

    constructor(props: any) {
        super(props);
        this.state = {
            videos: [],
            loading: true,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getAllVideos();
    }

    public getAllVideos = () => {
        this.setState({'loading': true});
        
        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            }
        };
        axios.get('/api/video/index', config)
            .then(res => {
                this.setState({'videos': res.data});
                this.setState({'loading': false});
            })
            .catch(err => {
                if(err.response.status == 401) {
                    this.setState({'unauthorized': true});
                }
            });
    };

    public render() {

        const progressBar = <img src="assets/loading.gif" alt="Loading..."/>

        return (
            <div className="container has-text-centered">

                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}
                
                {this.state.loading ? progressBar : null}
                <div className="columns is-multiline">
                    {this.state.videos.map((video) => {
                        return (
                            <Video 
                                videoId = {video.videoId}
                                title = {video.title}
                                videoPath = {video.videoPath}
                                transcription = {video.transcription}
                                dateAdded = {video.dateAdded}
                                key = {video.videoId}
                            />
                        )
                    })}
                </div>
            </div>
        )
    }

    

}
