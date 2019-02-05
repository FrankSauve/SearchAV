import * as React from 'react';
import FileInput from './FileInput';
import axios from 'axios';
import auth from '../../Utils/auth';
import FileTable from './FileTable';
import AutomatedFilter from './AutomatedFilter';
import EditedFilter from './EditedFilter';
import ReviewedFilter from './ReviewedFilter';
import MyFilesFilter from './MyFilesFilter';

interface State {
    loading: boolean,
    unauthorized: boolean
}

export default class Dashboard extends React.Component<any, State> {

    constructor(props: any) {
        super(props);

        this.state = {
            loading: false,
            unauthorized: false
        }
    }

    public render() {
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
                            <FileTable />
                        </div>
                    </section>
                </div>
            </div>
        )
    }
}
