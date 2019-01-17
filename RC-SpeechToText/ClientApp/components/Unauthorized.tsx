import * as React from 'react'

export default class Unauthorized extends React.Component<any> {
  render() {
    return (
      <div className="container">
        <h1 className="title is-1">401 ERROR: Unauthorized</h1>
        <h3 className="title is-3">You must log in to view this page</h3>
      </div>
    )
  }
}
