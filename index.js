require('dotenv').config()
require('ignore-styles')

require('@babel/register')({
  ignore: [/(node_modules)/],
  presets: ['@babel/preset-env', '@babel/preset-react'],
  plugins: ['@babel/plugin-proposal-class-properties']
})
if(process.argv.indexOf("prod") >=0 )
process.env.NODE_ENV = "production";

// cluster 
require('./scripts/cluster')();
