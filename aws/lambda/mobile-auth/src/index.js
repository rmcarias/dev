'use strict'
const AWS = require('aws-sdk');
const Promise = require('bluebird');

exports.handler = function (event, context, callback) {
    console.log(JSON.stringify(`Event: event`))
    console.log('testing this simple lambda')
    // Lambda Code Here
     context.succeed('Success!')
    // context.fail('Failed!')
}