import './css/site.css';
import 'bootstrap';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { BrowserRouter, Link } from 'react-router-dom';
import * as RoutesModule from './routes';
import { RouteComponentProps } from 'react-router';
import { VideoData } from './GetVideo';
let routes = RoutesModule.routes;

interface AddVideoDataState {
    title: string;
    loading: boolean;
    vidData: VideoData;
} 

export class AddVideo extends React.Component<RouteComponentProps<{}>, AddVideoDataState>
{

    constructor() {
        super();

        this.state = { title: "", loading: true, vidData: new VideoData };

        this.state = { title: "Create", loading: false, vidData: new VideoData }

        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    public render() {
        let contents = this.state.loading;
        return <div>
            <h1>{this.state.title}</h1>
            <h3>Video</h3>
            <hr />
            {contents}
        </div>;
    } 

    private handleSave(event: { preventDefault: () => void; target: HTMLFormElement | undefined; }) {
        event.preventDefault();
        const data = new FormData(event.target);

        fetch('api/Video/Add', {
            method: 'POST',
            body: data,

        }).then((response) => response.json())
            .then((responseJson) => {
                this.props.history.push("/GetVideo");
            })
    }
    private handleCancel(e: React.MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        this.props.history.push("/GetVideo");
    }

    private renderAddForm() {
        return (
            <form >
                <div className="form-group row" >
                    <input type="hidden" name="VideoId" value={this.state.vidData.videoId} />
                </div>
                < div className="form-group row" >
                    <label className=" control-label col-md-12" htmlFor="Name">Name</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="name" defaultValue={this.state.vidData.name} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Path" >Path</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="Path" defaultValue={this.state.vidData.path} required />
                    </div>
                </div>
                <div className="form-group">
                    <button type="submit" className="btn btn-default" onClick={(event) => { this.handleCancel(event) }}>Save</button> 
                    <button onClick={(e) => {this.handleCancel(e)}}>Cancel</button>
                </div >
            </form >
        )
    }




    
}