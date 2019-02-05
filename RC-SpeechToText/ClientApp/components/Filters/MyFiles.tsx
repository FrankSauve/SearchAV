import * as React from 'react';
import FileInput from '../Dashboard/FileInput';
import axios from 'axios';
import auth from '../../Utils/auth';
import FileTable from './FileTable';
import File from '../Dashboard//File';
import { Redirect } from 'react-router-dom';
import AutomatedFilter from '../Dashboard/AutomatedFilter';
import EditedFilter from '../Dashboard//EditedFilter';
import ReviewedFilter from '../Dashboard//ReviewedFilter';
import MyFilesFilter from '../Dashboard//MyFilesFilter';

interface State {
    userId: number,
    files: any[],
    loading: boolean,
    unauthorized: boolean
}

export default class MyFiles extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            userId: this.props.match.params.id,
            files: [],
            loading: false,
            unauthorized: false
        }
    }

    // Called when the component gets rendered
    public componentDidMount() {
        this.getUserFiles();
    }

    public getUserFiles = () => {

        const config = {
            headers: {
                'Authorization': 'Bearer ' + auth.getAuthToken(),
                'content-type': 'application/json'
            },
        };

        axios.get('/api/file/getAllFilesByUser/' + this.state.userId, config)
            .then(res => {
                console.log(res);
                this.setState({ 'files': res.data })
            })
            .catch(err => {
                if (err.response.status == 401) {
                    this.setState({ 'unauthorized': true });
                }
            });
    }

    public render() {

        const progressBar = <img src="assets/loading.gif" alt="Loading..." />

        var i = 0;

        return (
            <div className="container">
                <div className="columns">
                    <div className="column is-one-fifth">
                        <FileInput />
                        <br /> <br />
                        <AutomatedFilter />
                        <br />
                        <EditedFilter />
                        <br />
                        <ReviewedFilter />
                        <br /> <br />
                        <MyFilesFilter
                        />
                    </div>
                    <section className="section column">
                        <div className="box">
                            <div>

                                {this.state.unauthorized ? <Redirect to="/unauthorized" /> : null}

                                {this.state.loading ? progressBar : null}
                                <div className="columns is-multiline">
                                    {this.state.files.map((file) => {
                                        const FileComponent = <File
                                            fileId={file.id}
                                            flag={file.flag}
                                            title={file.title}
                                            description={file.description}
                                            username={auth.getName()}
                                            filePath={file.filePath}
                                            dateAdded={file.dateAdded}
                                            key={file.fileId}
                                        />
                                        i++;
                                        return (
                                            FileComponent
                                        )

                                    })}
                                </div>
                            </div>
                        </div>
                    </section>
                </div>
            </div>
        )
    }
}
