import * as React from 'react';
import FileInput from './FileInput';
import axios from 'axios';
import auth from '../../Utils/auth';
import FileTable from './FileTable';
import AutomatedFilter from './AutomatedFilter';

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
                        { /*<EditedFilter />
                        <ReviewedFilter />
                        <MyFilesFilter />*/}
                    </div>
                    <section className="section">
                        <div className="box">
                            <FileTable />
                        </div>
                    </section>
                </div>
            </div>
        )
    }
}
